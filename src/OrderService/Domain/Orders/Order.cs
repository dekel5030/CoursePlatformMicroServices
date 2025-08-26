using Domain.Orders.Errors;
using Domain.Orders.Events;
using Kernel;
using SharedKernel;
using SharedKernel.Customers;
using SharedKernel.Orders;

namespace Domain.Orders;

public class Order : Entity
{
    private readonly HashSet<LineItem> _items = new();

    private Order() {}

    public OrderId Id { get; private set; } = new(Guid.NewGuid());
    public CustomerId CustomerId { get; private set; }
    public Money TotalPrice { get; private set; } = Money.Zero();
    public IReadOnlyCollection<LineItem> Lines => _items;

    public static Result<Order> Create(CustomerId customerId)
    {
        var order = new Order { CustomerId = customerId, TotalPrice = Money.Zero() };

        order.Raise(new OrderDraftOpened(order.Id, customerId));
        return Result.Success(order);
    }

    public Result AddLine(LineItem item)
    {
        if (item is null) return Result.Failure(LineItemErrors.InvalidName);

        _items.Add(item);
        RecalculateTotal();
        Raise(new LineItemAdded(Id, item.Id, item.Quantity, item.UnitPrice));

        return Result.Success();
    }

    public Result<Order> Submit()
    {
        if (_items.Count == 0)
            return Result.Failure<Order>(OrderErrors.OrderIsEmpty);

        Raise(new OrderSubmitted(Id, CustomerId));

        return Result.Success(this);
    }

    private void RecalculateTotal()
    {
        if (_items.Count == 0) { TotalPrice = Money.Zero(TotalPrice.Currency); return; }
        var sum = _items
            .Select(i => i.TotalPrice)
            .Aggregate(Money.Zero(_items.First().UnitPrice.Currency), (acc, m) => acc + m);
        TotalPrice = sum;
    }
}