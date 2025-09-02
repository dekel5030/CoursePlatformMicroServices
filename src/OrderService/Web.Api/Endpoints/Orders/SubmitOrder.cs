
using Application.Abstractions.Messaging;
using Application.Orders.Commands.SubmitOrder;
using Domain.Orders.Primitives;
using Kernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Orders;

public class SubmitOrder : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("api/orders", async (
            SubmitOrderDto dto,
            ICommandHandler<SubmitOrderCommand, OrderId> handler,
            CancellationToken cancellationToken) =>
        {
            Result<OrderId> result = await handler.Handle(new SubmitOrderCommand(dto), cancellationToken);

            return result.Match(
                id => Results.CreatedAtRoute(
                    routeName: "GetOrderById",
                    routeValues: new { id = id.Value },
                    value: new { id = id.Value }
                ),
                CustomResults.Problem
            );
        })
        .WithTags(Tags.Orders);
    }
}
