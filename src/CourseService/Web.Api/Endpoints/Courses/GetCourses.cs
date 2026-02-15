using CoursePlatform.ServiceDefaults.CustomResults;
using CoursePlatform.ServiceDefaults.Swagger;
using Courses.Api.Extensions;
using Courses.Application.Features.CourseCatalog;
using Courses.Application.Shared.Dtos;
using Kernel;
using Kernel.Messaging.Abstractions;

namespace Courses.Api.Endpoints.Courses;

internal sealed class GetCourses : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("courses", async (
            [AsParameters] PagedQueryDto pagedQuery,
            IMediator mediator,
            HttpContext context,
            CancellationToken cancellationToken) =>
        {
            var query = new CourseCatalogQuery(pagedQuery);
            Result<CourseCatalogDto> result = await mediator.Send(query, cancellationToken);

            return result.Match(
                dto => Results.Ok(dto),
                CustomResults.Problem);
        })
        .WithMetadata<CourseCatalogDto>(
            nameof(GetCourses),
            tag: Tags.Courses,
            summary: "Gets a paginated list of courses.");
    }
}
