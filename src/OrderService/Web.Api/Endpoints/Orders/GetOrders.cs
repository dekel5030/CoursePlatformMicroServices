
using Application.Abstractions.Messaging;
using Application.Orders.Queries.Dtos;
using Application.Orders.Queries.GetOrders;
using Microsoft.AspNetCore.Mvc;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Orders;

public sealed class GetOrders : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/orders", async (
            [AsParameters] PaginationParams paginationDto,
            IQueryHandler<GetOrdersQuery, PagedResponse<OrderReadDto>> queryHandler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetOrdersQuery(paginationDto);
            var result = await queryHandler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Orders);
    }
}
