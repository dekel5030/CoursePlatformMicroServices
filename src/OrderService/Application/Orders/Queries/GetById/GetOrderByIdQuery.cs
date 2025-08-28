using Application.Abstractions.Messaging;
using Application.Orders.Queries.Dtos;
using Domain.Orders.Primitives;
using SharedKernel;

namespace Application.Orders.Queries.GetById;

public sealed record GetOrderByIdQuery(OrderId OrderId) : IQuery<OrderReadDto>;