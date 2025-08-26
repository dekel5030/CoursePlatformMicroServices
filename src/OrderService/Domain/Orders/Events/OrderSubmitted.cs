using SharedKernel;
using SharedKernel.Customers;
using SharedKernel.Orders;

namespace Domain.Orders.Events;

public sealed record OrderSubmitted(OrderId Id, CustomerId CustomerId) : IDomainEvent;
