using Domain.Orders.Errors;
using Kernel;
using SharedKernel;
using SharedKernel.Orders;
using SharedKernel.Products;

namespace Domain.Orders;

public class LineItem
{
    public LineItemId Id { get; private set; }
    public ProductId ProductId { get; set; }
    public decimal Quantity { get; private set; }
    public string Name { get; private set; } = null!;
    public Money UnitPrice { get; private set; } = null!;
    public Money TotalPrice => UnitPrice.Multiply(Quantity);

    private LineItem() { }

    public static Result<LineItem> Create(
        ProductId productId,
        decimal quantity,
        string name,
        Money unitPrice)
    {
        if (quantity <= 0) return Result.Failure<LineItem>(LineItemErrors.InvalidQuantity);

        if (string.IsNullOrWhiteSpace(name)) return Result.Failure<LineItem>(LineItemErrors.InvalidName);

        if (unitPrice.Amount < 0) return Result.Failure<LineItem>(LineItemErrors.InvalidPrice);

        var item = new LineItem()
        {
            Id = new LineItemId(Guid.NewGuid()),
            ProductId = productId,
            Quantity = quantity,
            Name = name,
            UnitPrice = unitPrice
        };

        return Result.Success(item);
    }
}
