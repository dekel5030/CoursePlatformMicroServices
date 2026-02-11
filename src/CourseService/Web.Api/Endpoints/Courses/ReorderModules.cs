using CoursePlatform.ServiceDefaults.CustomResults;
using CoursePlatform.ServiceDefaults.Swagger;
using Courses.Api.Extensions;
using Courses.Application.Modules.Commands.ReorderModules;
using Courses.Domain.Courses.Primitives;
using Courses.Domain.Modules.Primitives;
using Kernel;
using Kernel.Messaging.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Courses.Api.Endpoints.Courses;

internal sealed class ReorderModules : IEndpoint
{
    internal sealed record ReorderModulesRequest(List<Guid> ModuleIds);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("courses/{courseId:Guid}/structure/modules", async (
            Guid courseId,
            ReorderModulesRequest request,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var moduleIds = request.ModuleIds
                .Select(id => new ModuleId(id))
                .ToList();

            ReorderModulesCommand command = new(new CourseId(courseId), moduleIds);

            Result result = await mediator.Send(command, cancellationToken);

            return result.Match(
                () => Results.NoContent(),
                CustomResults.Problem);
        })
        .WithMetadata<EmptyResult>(
            nameof(ReorderModules),
            Tags.Courses,
            "Reorders modules within a course.");
    }
}
