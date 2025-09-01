using Domain.Orders.Primitives;
using Domain.Users;
using SharedKernel;

namespace Domain.Orders.Events;

public sealed record OrderSubmittedDomainEvent(OrderId Id, UserId UserId, OrderStatus Status) : IDomainEvent;
