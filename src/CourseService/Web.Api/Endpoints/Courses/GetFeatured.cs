using CoursePlatform.ServiceDefaults.CustomResults;
using CoursePlatform.ServiceDefaults.Swagger;
using Courses.Api.Contracts.Courses;
using Courses.Api.Contracts.Shared;
using Courses.Api.Extensions;
using Courses.Api.Infrastructure.LinkProvider;
using Courses.Application.Courses.Queries.Dtos;
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
            LinkProvider linkProvider,
            CancellationToken cancellationToken) =>
        {
            Result<PagedResponseDto<CourseSummaryDto>> result = await mediator
                .Send(new GetFeaturedQuery(), cancellationToken);

            return result.Match(
                dto => Results.Ok(dto.ToApiContract(linkProvider)),
                CustomResults.Problem);
        })
        .WithMetadata<PagedResponse<CourseSummaryResponse>>(
            nameof(GetFeatured),
            tag: Tags.Courses,
            summary: "Gets featured courses.");
    }
}
