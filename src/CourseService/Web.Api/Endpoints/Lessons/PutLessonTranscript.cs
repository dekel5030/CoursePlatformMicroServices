using CoursePlatform.ServiceDefaults.CustomResults;
using CoursePlatform.ServiceDefaults.Swagger;
using Courses.Api.Extensions;
using Courses.Application.Lessons.Commands.UpdateTranscript;
using Courses.Application.Lessons.Queries.GetTranscript;
using Kernel;
using Kernel.Messaging.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Courses.Api.Endpoints.Lessons;

internal sealed class PutLessonTranscript : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("lessons/{lessonId:Guid}/transcript", async (
            Guid lessonId,
            List<TranscriptSegmentDto> segments,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var command = new UpdateTranscriptCommand(lessonId, segments ?? new List<TranscriptSegmentDto>());
            Result result = await mediator.Send(command, cancellationToken);

            return result.Match(
                () => Results.NoContent(),
                CustomResults.Problem);
        })
        .WithName(nameof(PutLessonTranscript))
        .WithMetadata<EmptyResult>(
            nameof(PutLessonTranscript),
            tag: Tags.Lessons,
            summary: "Updates the lesson transcript (JSON segments serialized to VTT and stored in S3).")
        .RequireAuthorization();
    }
}
