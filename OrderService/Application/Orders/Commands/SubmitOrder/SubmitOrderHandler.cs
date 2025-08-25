using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Orders;
using Domain.Orders.Errors;
using SharedKernel;
using SharedKernel.Customers;
using SharedKernel.Products;

namespace Application.Orders.Commands.SubmitOrder;

public sealed class SubmitOrderCommandHandler : ICommandHandler<SubmitOrderCommand, Guid>
{
    private readonly IApplicationDbContext _dbContext;

    public SubmitOrderCommandHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<Guid>> Handle(SubmitOrderCommand command, CancellationToken cancellationToken)
    {
        SubmitOrderDto dto = command.Dto;
        //if (await _dbContext.Customers.FindAsync(dto.CustomerId, cancellationToken) is null)
        //{
        //    return Result.Failure(OrderErrors.CustomerNotFound);
        //}

        Result<Order> result = Order.Create(new CustomerId(dto.CustomerId));

        if (result.IsFailure)
        {
            return Result.Failure<Guid>(result.Error!);
        }

        Order order = result.Value;

        foreach (SubmitOrderItemDto itemDto in dto.Products)
        {
            //Product product = await _dbContext.Products.AsNoTracking()
            //    .SingleOrDefaultAsync(x => x.Id == productId, cancellationToken);

            //if (product is null)
            //{
            //    return Result.Failure(ProductErrors.ProductNotFound);
            //}

            //Resukt<LineItem> lineItemResult = LineItem.Create(
            //    product.Id,
            //    quantity,
            //    new Sku(product.Id),
            //    product.Name,
            //    product.Price);


            Result<LineItem> lineItemResult = LineItem.Create(
                new ProductId(itemDto.Id),
                itemDto.Quantity,
                new Sku(itemDto.Id.ToString()),
                "name",
                Money.Zero());

            if (lineItemResult.IsFailure)
            {
                return Result.Failure<Guid>(lineItemResult.Error!);
            }

            order.AddLine(lineItemResult.Value);
        }

        await _dbContext.Orders.AddAsync(order, cancellationToken);
        await _dbContext.SaveChangesAsync();
        order.Submit();
        
        return Result.Success(order.Id.Value);
    }
}

