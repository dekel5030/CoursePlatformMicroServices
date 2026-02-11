using CoursePlatform.ServiceDefaults.CustomResults;
using CoursePlatform.ServiceDefaults.Swagger;
using Courses.Api.Extensions;
using Courses.Application.Lessons.Commands.ReorderLessons;
using Courses.Domain.Lessons.Primitives;
using Courses.Domain.Modules.Primitives;
using Kernel;
using Kernel.Messaging.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Courses.Api.Endpoints.Modules;

internal sealed class ReorderLessons : IEndpoint
{
    internal sealed record ReorderLessonsRequest(List<Guid> LessonIds);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("modules/{moduleId:Guid}/lessons/reorder", async (
            Guid moduleId,
            ReorderLessonsRequest request,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var lessonIds = request.LessonIds
                .Select(id => new LessonId(id))
                .ToList();

            ReorderLessonsCommand command = new(new ModuleId(moduleId), lessonIds);

            Result result = await mediator.Send(command, cancellationToken);

            return result.Match(
                () => Results.NoContent(),
                CustomResults.Problem);
        })
        .WithMetadata<EmptyResult>(
            nameof(ReorderLessons),
            Tags.Modules,
            "Reorders lessons within a module.");
    }
}
