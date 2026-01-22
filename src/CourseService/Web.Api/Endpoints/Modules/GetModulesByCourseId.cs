using CoursePlatform.ServiceDefaults.CustomResults;
using CoursePlatform.ServiceDefaults.Swagger;
using Courses.Api.Contracts.Modules;
using Courses.Api.Contracts.Shared;
using Courses.Api.Extensions;
using Courses.Api.Infrastructure.LinkProvider;
using Courses.Application.Modules.Dtos;
using Courses.Application.Modules.Queries.GetByCourseId;
using Courses.Domain.Courses.Primitives;
using Kernel;
using Kernel.Messaging.Abstractions;

namespace Courses.Api.Endpoints.Modules;

internal sealed class GetModulesByCourseId : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("courses/{courseId:Guid}/modules", async (
            Guid courseId,
            IMediator mediator,
            LinkProvider linkProvider,
            CancellationToken cancellationToken) =>
        {
            var query = new GetModulesByCourseIdQuery(new CourseId(courseId));

            Result<ModuleCollectionDto> result = await mediator.Send(query, cancellationToken);

            var courseIdObj = new CourseId(courseId);

            return result.Match(
                dto => Results.Ok(dto.ToApiContract(courseIdObj, linkProvider)),
                CustomResults.Problem);
        })
        .WithMetadata<PagedResponse<ModuleDetailsResponse>>(
            nameof(GetModulesByCourseId),
            tag: Tags.Modules,
            summary: "Gets all modules for a course by course ID.");
    }
}
