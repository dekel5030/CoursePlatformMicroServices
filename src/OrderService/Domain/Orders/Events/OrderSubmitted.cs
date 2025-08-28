using Domain.Orders.Primitives;
using Domain.Users;
using SharedKernel;

namespace Domain.Orders.Events;

public sealed record OrderSubmitted(OrderId Id, UserId CustomerId) : IDomainEvent;
