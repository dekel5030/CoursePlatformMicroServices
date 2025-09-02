using Domain.Orders.Errors;
using Domain.Orders.Events;
using Domain.Orders.Primitives;
using Domain.Users.Primitives;
using Kernel;
using SharedKernel;

namespace Domain.Orders;

public class Order : Entity
{
    private readonly HashSet<LineItem> _items = new();

    private Order() {}

    public OrderId Id { get; private set; } = new(Guid.CreateVersion7());
    public ExternalUserId ExternalUserId { get; private set; }
    public OrderStatus Status { get; private set; }
    public Money TotalPrice { get; private set; } = Money.Zero();
    public IReadOnlyCollection<LineItem> Lines => _items;

    public static Order Create(ExternalUserId externalUserId)
    {
        var order = new Order { ExternalUserId = externalUserId, TotalPrice = Money.Zero() };

        order.Raise(new OrderDraftOpened(order.Id, externalUserId));
        return order;
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

        Status = OrderStatus.Submitted;

        Raise(new OrderSubmittedDomainEvent(Id, ExternalUserId, Status));

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