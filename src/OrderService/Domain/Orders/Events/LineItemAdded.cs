using SharedKernel;
using Domain.Orders.Primitives;

namespace Domain.Orders.Events;

public sealed record LineItemAdded(
    OrderId OrderId, LineItemId LineItemId, decimal Quantity, Money UnitPrice) : IDomainEvent;