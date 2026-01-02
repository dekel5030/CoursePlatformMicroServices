
using CoursePlatform.ServiceDefaults.CustomResults;
using Courses.Api.Extensions;
using Courses.Application.Courses.Queries.Dtos;
using Courses.Application.Courses.Queries.GetFeatured;
using Kernel.Messaging.Abstractions;

namespace Courses.Api.Endpoints.Courses;

public class GetFeatured : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("courses/featured", async (
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(new GetFeaturedQuery(), cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        });
    }
}
