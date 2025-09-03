using Domain.Orders.Errors;
using Domain.Orders.Primitives;
using Domain.Products.Primitives;
using Kernel;
using SharedKernel;

namespace Domain.Orders;

public class LineItem
{
    public LineItemId Id { get; private set; }
    public ExternalProductId ExternalProductId { get; set; }
    public decimal Quantity { get; private set; }
    public string Name { get; private set; } = null!;
    public Money UnitPrice { get; private set; } = null!;
    public Money TotalPrice => UnitPrice.Multiply(Quantity);

    private LineItem() { }

    public static Result<LineItem> Create(
        ExternalProductId externalProductId,
        decimal quantity,
        string name,
        Money unitPrice)
    {
        if (quantity <= 0) return Result.Failure<LineItem>(LineItemErrors.InvalidQuantity);

        if (string.IsNullOrWhiteSpace(name)) return Result.Failure<LineItem>(LineItemErrors.IsNull);

        if (unitPrice.Amount < 0) return Result.Failure<LineItem>(LineItemErrors.InvalidPrice);

        var item = new LineItem()
        {
            Id = new LineItemId(Guid.CreateVersion7()),
            ExternalProductId = externalProductId,
            Quantity = quantity,
            Name = name,
            UnitPrice = unitPrice
        };

        return Result.Success(item);
    }
}
