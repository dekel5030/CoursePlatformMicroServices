using CoursePlatform.ServiceDefaults.CustomResults;
using CoursePlatform.ServiceDefaults.Swagger;
using Courses.Api.Extensions;
using Courses.Application.Modules.Dtos;
using Courses.Application.Modules.Queries.GetModules;
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
            CancellationToken cancellationToken) =>
        {
            var query = new GetModulesQuery(new ModuleFilter(CourseId: new CourseId(courseId)));

            Result<IReadOnlyList<ModuleDto>> result = await mediator.Send(query, cancellationToken);

            return result.Match(
                dto => Results.Ok(dto),
                CustomResults.Problem);
        })
        .WithMetadata<IReadOnlyList<ModuleDto>>(
            nameof(GetModulesByCourseId),
            tag: Tags.Modules,
            summary: "Gets all modules for a course by course ID.");
    }
}
