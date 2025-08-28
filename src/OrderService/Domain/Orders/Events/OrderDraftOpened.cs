using Domain.Orders.Primitives;
using Domain.Users;
using SharedKernel;

namespace Domain.Orders.Events;

public sealed record OrderDraftOpened(OrderId OrderId, UserId CustomerId) : IDomainEvent;
