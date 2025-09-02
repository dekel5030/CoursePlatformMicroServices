using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Orders;
using Domain.Orders.Errors;
using Domain.Orders.Events;
using Domain.Orders.Primitives;
using Domain.Products;
using Domain.Users.Errors;
using Domain.Users.Primitives;
using Kernel;
using Microsoft.EntityFrameworkCore;
using SharedKernel;
using SharedKernel.Products;

namespace Application.Orders.Commands.SubmitOrder;

public sealed class SubmitOrderCommandHandler : ICommandHandler<SubmitOrderCommand, OrderId>
{
    private readonly IApplicationDbContext _dbContext;

    public SubmitOrderCommandHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<OrderId>> Handle(SubmitOrderCommand command, CancellationToken cancellationToken)
    {
        SubmitOrderDto dto = command.Dto;
        var externalUserId = new ExternalUserId(dto.ExternalUserId);

        bool userExists = await _dbContext.Users
            .AsNoTracking()
            .AnyAsync(user => user.ExternalUserId == externalUserId, cancellationToken);

        if (!userExists)
        {
            return Result.Failure<OrderId>(UserErrors.NotFound);
        }

        Order order = Order.Create(externalUserId);

        foreach (SubmitOrderItemDto itemDto in dto.Products)
        {
            //Product product = await _dbContext.Products.AsNoTracking()
            //    .SingleOrDefaultAsync(x => x.Id == productId, cancellationToken);

            //if (product is null)
            //{
            //    return Result.Failure(ProductErrors.ProductNotFound);
            //}

            Result<LineItem> lineItemResult = LineItem.Create(
                new ProductId(itemDto.Id),
                itemDto.Quantity,
                "name",
                Money.Zero());

            if (lineItemResult.IsFailure)
            {
                return Result.Failure<OrderId>(lineItemResult.Error!);
            }

            order.AddLine(lineItemResult.Value);
        }

        await _dbContext.Orders.AddAsync(order, cancellationToken);
        Result<Order> submitResult = order.Submit();


        if (submitResult.IsFailure)
        {
            return Result.Failure<OrderId>(submitResult.Error!);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(order.Id);
    }
}