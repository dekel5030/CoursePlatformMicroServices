using SharedKernel.Orders;
using SharedKernel.Products;

namespace Domain.Orders;

public class LineItem
{
    public LineItemId Id { get; private set; }
    public ProductId ProductId { get; set; }
    public decimal Quantity { get; private set; }
    public Sku Sku { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public Money UnitPrice { get; private set; } = null!;
    public Money TotalPrice => UnitPrice.Multiply(Quantity);

    private LineItem() { }

    public static LineItem? Create(
        ProductId productId,
        decimal quantity,
        Sku sku,
        string name,
        Money unitPrice)
    {
        if (quantity <= 0 || unitPrice.Amount < 0 || string.IsNullOrWhiteSpace(name))
        {
            return null;
        }

        return new LineItem()
        {
            Id = new LineItemId(Guid.NewGuid()),
            ProductId = productId,
            Quantity = quantity,
            Sku = sku,
            Name = name,
            UnitPrice = unitPrice
        };
    }
}
