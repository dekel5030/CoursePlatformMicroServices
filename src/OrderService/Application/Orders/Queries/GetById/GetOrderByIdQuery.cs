using Application.Abstractions.Messaging;
using Application.Orders.Queries.Dtos;
using SharedKernel;
using SharedKernel.Orders;

namespace Application.Orders.Queries.GetById;

public sealed record GetOrderByIdQuery(OrderId OrderId) : IQuery<OrderReadDto>;