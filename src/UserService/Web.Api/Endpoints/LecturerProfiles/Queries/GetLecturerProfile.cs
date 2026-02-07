using Kernel;
using Kernel.Messaging.Abstractions;
using Users.Api.Extensions;
using Users.Api.Infrastructure;
using Users.Application.LecturerProfiles.Queries.Dtos;
using Users.Application.LecturerProfiles.Queries.GetLecturerProfile;

namespace Users.Api.Endpoints.LecturerProfiles.Queries;

internal sealed class GetLecturerProfile : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/users/{userId:guid}/lecturer-profile", async (
            Guid userId,
            IQueryHandler<GetLecturerProfileQuery, LecturerProfileDto> handler,
            CancellationToken cancellationToken = default) =>
        {
            var query = new GetLecturerProfileQuery(userId);

            Result<LecturerProfileDto> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        }).RequireAuthorization();
    }
}
