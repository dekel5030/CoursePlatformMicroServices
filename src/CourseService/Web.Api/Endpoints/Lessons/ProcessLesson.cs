using CoursePlatform.ServiceDefaults.CustomResults;
using CoursePlatform.ServiceDefaults.Swagger;
using Courses.Api.Extensions;
using Courses.Application.Lessons.Commands.ProcessLesson;
using Courses.Domain.Lessons.Primitives;
using Kernel;
using Kernel.Messaging.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Courses.Api.Endpoints.Lessons;

internal sealed class ProcessLesson : IEndpoint
{
    internal sealed record ProcessLessonRequest(string Message);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("lessons/{lessonId:Guid}/process", async (
            Guid lessonId,
            ProcessLessonRequest request,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var command = new ProcessLessonCommand(
                new LessonId(lessonId),
                request.Message ?? string.Empty);

            Result result = await mediator.Send(command, cancellationToken);

            return result.Match(
                () => Results.NoContent(),
                CustomResults.Problem);
        })
        .WithMetadata<EmptyResult>(
            nameof(ProcessLesson),
            Tags.Lessons,
            "Triggers processing of a lesson's raw resources.",
            StatusCodes.Status204NoContent);
    }
}
