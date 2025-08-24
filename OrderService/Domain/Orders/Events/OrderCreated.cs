using SharedKernel;
using SharedKernel.Customers;
using SharedKernel.Orders;

namespace Domain.Orders.Events;

public sealed record OrderCreated(OrderId OrderId, CustomerId CustomerId) : IDomainEvent;
