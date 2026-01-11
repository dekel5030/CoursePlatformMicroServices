using Kernel;
using Kernel.Messaging.Abstractions;
using Users.Api.Extensions;
using Users.Api.Infrastructure;
using Users.Application.Users.Queries.Dtos;
using Users.Application.Users.Queries.GetUserByid;

namespace Users.Api.Endpoints.Users.Queries;

internal sealed class GetUserById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/users/{id:guid}", async (
            Guid id,
            IQueryHandler<GetUserByIdQuery, UserReadDto> handler,
            CancellationToken cancellationToken = default) =>
        {
            var query = new GetUserByIdQuery(id);
            Result<UserReadDto> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithName("GetUserById");
    }
}