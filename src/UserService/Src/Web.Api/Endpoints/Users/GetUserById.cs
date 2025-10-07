
using Application.Abstractions.Messaging;
using Application.Users.Queries.GetUserByid;
using Kernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Users;

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
    }
}
