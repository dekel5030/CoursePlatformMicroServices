using Domain.Orders;
using SharedKernel.Orders;

namespace Application.Abstractions;

public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(OrderId orderId, CancellationToken cancellationToken);
}