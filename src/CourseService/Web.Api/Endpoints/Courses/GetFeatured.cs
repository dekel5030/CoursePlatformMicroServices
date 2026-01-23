using CoursePlatform.ServiceDefaults.CustomResults;
using CoursePlatform.ServiceDefaults.Swagger;
using Courses.Api.Endpoints.Contracts.Courses;
using Courses.Api.Endpoints.Contracts.Shared;
using Courses.Api.Extensions;
using Courses.Api.Infrastructure.LinkProvider;
using Courses.Application.Courses.Dtos;
using Courses.Application.Courses.Queries.GetFeatured;
using Courses.Application.Shared.Dtos;
using Kernel;
using Kernel.Messaging.Abstractions;

namespace Courses.Api.Endpoints.Courses;

internal sealed class GetFeatured : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("courses/featured", async (
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var query = new GetFeaturedQuery();
            Result<CourseCollectionDto> result = await mediator.Send(query, cancellationToken);

            return result.Match(
                dto => Results.Ok(dto),
                CustomResults.Problem);
        })
        .WithMetadata<CourseCollectionDto>(
            nameof(GetFeatured),
            tag: Tags.Courses,
            summary: "Gets featured courses.");
    }
}
