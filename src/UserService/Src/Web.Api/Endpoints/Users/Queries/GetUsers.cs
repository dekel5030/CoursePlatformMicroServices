﻿
using Application.Abstractions.Messaging;
using Application.Users.Queries.Dtos;
using Application.Users.Queries.GetUserByid;
using Application.Users.Queries.GetUsers;
using Kernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Users;

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
