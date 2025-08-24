using Application.Abstractions.Messaging;
using Application.Orders.Queries.Dtos;
using Application.Orders.Queries.GetById;
using SharedKernel;
using SharedKernel.Orders;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Orders;

internal sealed class GetById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("Orders/{orderId}", async (
            Guid orderId,
            IQueryHandler<GetOrderByIdQuery, OrderReadDto> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetOrderByIdQuery(new OrderId(orderId));

            Result<OrderReadDto> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Orders);
    }
}
