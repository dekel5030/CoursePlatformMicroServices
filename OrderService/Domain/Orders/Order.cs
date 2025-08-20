using SharedKernel.Customers;
using SharedKernel.Orders;

namespace Domain.Orders;

public class Order
{
    private readonly HashSet<LineItem> _orderItems = new HashSet<LineItem>();

    public OrderId Id { get; private set; }
    public CustomerId customerId { get; private set; }
    public Money Price { get; private set; } = Money.Zero();
    public IReadOnlyList<LineItem> Lines => _orderItems.ToList().AsReadOnly();

    private Order() { }

    public static Order Create(CustomerId customerId)
    {
        return new Order()
        {
            Id = new OrderId(Guid.NewGuid()),
            customerId = customerId
        };
    }

    public void Add(LineItem orderItem)
    {
        if (orderItem == null)
        {
            throw new ArgumentNullException(nameof(orderItem), "Order item cannot be null");
        }

        _orderItems.Add(orderItem);
    }
}