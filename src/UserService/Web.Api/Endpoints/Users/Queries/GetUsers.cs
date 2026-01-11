using Kernel;
using Kernel.Messaging.Abstractions;
using Users.Api.Extensions;
using Users.Api.Infrastructure;
using Users.Application.Users.Queries.Dtos;
using Users.Application.Users.Queries.GetUsers;

namespace Users.Api.Endpoints.Users.Queries;

internal sealed class GetUser : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/users/", async (
            IQueryHandler<GetUsersQuery, CollectionDto<UserReadDto>> handler,
            [AsParameters] GetUsersQuery query,
            CancellationToken cancellationToken = default) =>
        {
            Result<CollectionDto<UserReadDto>> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        });
    }
}