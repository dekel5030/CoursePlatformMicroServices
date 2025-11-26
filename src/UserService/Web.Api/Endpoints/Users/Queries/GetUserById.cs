
using Application.Abstractions.Messaging;
using Application.Users.Queries.Dtos;
using Application.Users.Queries.GetUserByid;
using Kernel;
using User.Api.Endpoints;
using User.Api.Extensions;
using User.Api.Infrastructure;

namespace User.Api.Endpoints.Users.Queries;

public class GetUserById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/users/{id:guid}", async (
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
