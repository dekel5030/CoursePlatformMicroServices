using SharedKernel.Orders;
using SharedKernel;

namespace Domain.Orders.Events;

public sealed record LineItemAdded(
    OrderId OrderId, LineItemId LineItemId, decimal Quantity, Money UnitPrice) : IDomainEvent;