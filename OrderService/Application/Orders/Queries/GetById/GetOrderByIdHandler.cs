using Application.Abstractions.Messaging;
using Application.Orders.Queries.Dtos;
using Domain.Orders;

namespace Application.Orders.Queries.GetById;

public sealed class GetOrderByIdHandler
    : IQueryHandler<GetOrderByIdQuery, OrderReadDto?>
{
    private readonly IOrderRepository _orders;

    public GetOrderByIdHandler(IOrderRepository orders) => _orders = orders;

    public async Task<OrderReadDto?> Handle(GetOrderByIdQuery q, CancellationToken cancellationToken = default)
    {
        Order? order = await _orders.GetByIdAsync(q.OrderId, cancellationToken);

        if (order is null) return null;

        string currency = order.Lines.FirstOrDefault()?.UnitPrice.Currency ?? "ILS";
        Money totalAmount = order.Price;

        List<LineItemReadDto> lines = order.Lines
            .Select(l => new LineItemReadDto(l.ProductId, l.Quantity, l.Name, l.UnitPrice, l.TotalPrice))
            .ToList();

        return new OrderReadDto(order.Id, order.customerId, currency, totalAmount, lines);
    }
}
