using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Orders;
using Domain.Orders.Primitives;
using Domain.Products;
using Domain.Products.Errors;
using Domain.Products.Primitives;
using Domain.Users.Errors;
using Domain.Users.Primitives;
using Kernel;
using Microsoft.EntityFrameworkCore;

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
        var dto = command.Dto;
        var externalUserId = new ExternalUserId(dto.ExternalUserId);

        if (!await UserExists(externalUserId, cancellationToken))
            return Result.Failure<OrderId>(UserErrors.NotFound);

        var order = Order.Create(externalUserId);

        var productsResult = await GetProducts(dto.Products, cancellationToken);
        if (productsResult.IsFailure)
            return Result.Failure<OrderId>(productsResult.Error!);

        var lineItemsResult = CreateLineItems(dto.Products, productsResult.Value);
        if (lineItemsResult.IsFailure)
            return Result.Failure<OrderId>(lineItemsResult.Error!);

        order.AddLines(lineItemsResult.Value);

        await _dbContext.Orders.AddAsync(order, cancellationToken);

        var submitResult = order.Submit();
        if (submitResult.IsFailure)
            return Result.Failure<OrderId>(submitResult.Error!);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(order.Id);
    }

    private async Task<bool> UserExists(ExternalUserId externalUserId, CancellationToken cancellationToken)
    {
        return await _dbContext.Users
            .AsNoTracking()
            .AnyAsync(user => user.ExternalUserId == externalUserId, cancellationToken);
    }

    private async Task<Result<Dictionary<ExternalProductId, Product>>> GetProducts(
        IReadOnlyList<ProductDto> items,
        CancellationToken cancellationToken)
    {
        var externalProductIds = items.Select(p => new ExternalProductId(p.ExternalId)).ToList();

        var products = await _dbContext.Products
            .AsNoTracking()
            .Where(p => externalProductIds.Contains(p.ExternalId))
            .ToListAsync(cancellationToken);

        if (products.Count != externalProductIds.Count)
            return Result.Failure<Dictionary<ExternalProductId, Product>>(ProductErrors.NotFound);

        var productsById = products.ToDictionary(p => p.ExternalId);
        return Result.Success(productsById);
    }

    private Result<List<LineItem>> CreateLineItems(
        IReadOnlyList<ProductDto> items,
        Dictionary<ExternalProductId, Product> productsById)
    {
        var lineItems = new List<LineItem>();

        foreach (var itemDto in items)
        {
            var externalProductId = new ExternalProductId(itemDto.ExternalId);

            if (!productsById.TryGetValue(externalProductId, out var product))
                return Result.Failure<List<LineItem>>(ProductErrors.NotFound);

            var lineItemResult = LineItem.Create(
                externalProductId,
                itemDto.Quantity,
                product.Name,
                product.Price
            );

            if (lineItemResult.IsFailure)
                return Result.Failure<List<LineItem>>(lineItemResult.Error!);

            lineItems.Add(lineItemResult.Value);
        }

        return Result.Success(lineItems);
    }
}
