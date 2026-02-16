using CoursePlatform.ServiceDefaults.CustomResults;
using CoursePlatform.ServiceDefaults.Swagger;
using Courses.Api.Extensions;
using Courses.Application.Lessons.Queries.GetTranscript;
using Kernel;
using Kernel.Messaging.Abstractions;

namespace Courses.Api.Endpoints.Lessons;

internal sealed class GetLessonTranscript : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("lessons/{lessonId:Guid}/transcript", async (
            Guid lessonId,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var query = new GetTranscriptQuery(lessonId);
            Result<IReadOnlyList<TranscriptSegmentDto>> result =
                await mediator.Send(query, cancellationToken);

            return result.Match(
                segments => Results.Ok(segments),
                CustomResults.Problem);
        })
        .WithName(nameof(GetLessonTranscript))
        .WithMetadata<IReadOnlyList<TranscriptSegmentDto>>(
            nameof(GetLessonTranscript),
            tag: Tags.Lessons,
            summary: "Gets the transcript segments for a lesson (VTT parsed to JSON).")
        .RequireAuthorization();
    }
}
