using Domain.Orders.Primitives;
using Domain.Users.Primitives;
using SharedKernel;

namespace Domain.Orders.Events;

public sealed record OrderSubmittedDomainEvent(OrderId Id, ExternalUserId UserId, OrderStatus Status) : IDomainEvent;
