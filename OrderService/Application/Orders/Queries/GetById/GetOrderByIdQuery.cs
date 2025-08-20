using Application.Abstractions;
using Application.Orders.Queries.Dtos;
using SharedKernel.Orders;

namespace Application.Orders.Queries.GetById;

public sealed record GetOrderByIdQuery(OrderId OrderId) : IQuery<OrderReadDto?>;