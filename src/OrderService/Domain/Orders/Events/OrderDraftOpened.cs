using Domain.Orders.Primitives;
using Domain.Users.Primitives;
using SharedKernel;

namespace Domain.Orders.Events;

public sealed record OrderDraftOpened(OrderId OrderId, ExternalUserId CustomerId) : IDomainEvent;
