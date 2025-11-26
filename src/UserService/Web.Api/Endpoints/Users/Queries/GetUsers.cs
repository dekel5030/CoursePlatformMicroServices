
using Application.Abstractions.Messaging;
using Application.Users.Queries.Dtos;
using Application.Users.Queries.GetUserByid;
using Application.Users.Queries.GetUsers;
using Kernel;
using User.Api.Endpoints;
using User.Api.Extensions;
using User.Api.Infrastructure;

namespace User.Api.Endpoints.Users.Queries;

public class GetUserById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/users/", async (
            IQueryHandler<GetUsersQuery, CollectionDto<UserReadDto>> handler,
            [AsParameters] GetUsersQuery query,
            CancellationToken cancellationToken = default) =>
        {
            Result<CollectionDto<UserReadDto>> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        });
    }
}
