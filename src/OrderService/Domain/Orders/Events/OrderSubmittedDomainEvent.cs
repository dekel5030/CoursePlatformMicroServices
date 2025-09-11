using Domain.Orders.Primitives;
using SharedKernel;

namespace Domain.Orders.Events;

public sealed record OrderSubmittedDomainEvent(
    OrderId Id, 
    long EntityVersion,
    OrderStatus Status) : IDomainEvent;
