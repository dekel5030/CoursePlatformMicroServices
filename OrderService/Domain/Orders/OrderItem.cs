using SharedKernel.Orders;
using SharedKernel.Products;

namespace Domain.Orders;

public class OrderItem
{
    public OrderItemId Id { get; private set; }
    public ProductId ProductId { get; private set; }
    public Money Money { get; private set; } = new Money(0m, "ILS");

    private OrderItem() { }

    public static OrderItem Create(OrderItemId id, ProductId productId, Money money)
    {
        return new OrderItem()
        {
            Id = id,
            ProductId = productId,
            Money = money
        };
    }
}
