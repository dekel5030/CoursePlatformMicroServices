using SharedKernel;
using SharedKernel.Customers;
using SharedKernel.Orders;

namespace Domain.Orders.Events;

public sealed record OrderDraftOpened(OrderId OrderId, CustomerId CustomerId) : IDomainEvent;
