
using Application.Abstractions.Messaging;
using Application.Orders.Commands.SubmitOrder;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Orders;

public class SubmitOrder : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("api/orders", async (
            SubmitOrderDto dto,
            ICommandHandler<SubmitOrderCommand, Guid> handler,
            CancellationToken cancellationToken) =>
        {
            Result<Guid> result = await handler.Handle(new SubmitOrderCommand(dto), cancellationToken);

            return result.Match(
                id => Results.CreatedAtRoute(
                    routeName: "GetOrderById",
                    routeValues: new { id },
                    value: new { id }
                ),
                CustomResults.Problem
            );
        })
        .WithTags(Tags.Orders);
    }
}
