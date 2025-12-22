
using Application.Users.Queries.Dtos;
using Application.Users.Queries.GetUsers;
using Kernel;
using Kernel.Messaging.Abstractions;
using User.Api.Extensions;
using User.Api.Infrastructure;

namespace User.Api.Endpoints.Users.Queries;

public class GetUser : IEndpoint
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