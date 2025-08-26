using Application.Abstractions.Messaging;
using Application.Orders.Queries.Dtos;

namespace Application.Orders.Queries.GetOrders;

public sealed record GetOrdersQuery(PaginationParams Pagination) : IQuery<PagedResponse<OrderReadDto>>;