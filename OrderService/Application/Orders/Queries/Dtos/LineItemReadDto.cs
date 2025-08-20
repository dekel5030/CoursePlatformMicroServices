using Domain.Orders;
using SharedKernel.Products;

namespace Application.Orders.Queries.Dtos;

public class LineItemReadDto
{
    public LineItemReadDto(ProductId productId, decimal quantity, string name, Money unitPrice, Money totalPrice)
    {
        ProductId = productId;
        Quantity = quantity;
        Name = name;
        UnitPrice = unitPrice;
        TotalPrice = totalPrice;
    }

    public ProductId ProductId { get; }
    public decimal Quantity { get; }
    public string Name { get; }
    public Money UnitPrice { get; }
    public Money TotalPrice { get; }
}
