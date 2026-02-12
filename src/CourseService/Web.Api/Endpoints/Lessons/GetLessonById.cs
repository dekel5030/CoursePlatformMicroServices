using CoursePlatform.ServiceDefaults.CustomResults;
using CoursePlatform.ServiceDefaults.Swagger;
using Courses.Api.Extensions;
using Courses.Application.Lessons.Dtos;
using Courses.Application.Lessons.Queries.GetLessons;
using Courses.Domain.Lessons.Errors;
using Kernel;
using Kernel.Messaging.Abstractions;

namespace Courses.Api.Endpoints.Lessons;

internal sealed class GetLessonById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("lessons/{lessonId:Guid}", async (
            Guid lessonId,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var query = new GetLessonsQuery(new LessonFilter(
                Ids: [lessonId],
                IncludeDetails: true));
            Result<IReadOnlyList<LessonDto>> result = await mediator.Send(query, cancellationToken);

            return result.Match(
                list =>
                {
                    LessonDto? lesson = list.Count > 0 ? list[0] : null;
                    if (lesson is null)
                    {
                        return CustomResults.Problem(Result.Failure<LessonDetailsPageDto>(LessonErrors.NotFound));
                    }
                    var pageDto = new LessonDetailsPageDto
                    {
                        LessonId = lesson.Id,
                        ModuleId = lesson.ModuleId ?? Guid.Empty,
                        CourseId = lesson.CourseId ?? Guid.Empty,
                        CourseName = lesson.CourseName ?? "",
                        Title = lesson.Title,
                        Description = lesson.Description ?? "",
                        Index = lesson.Index,
                        Duration = lesson.Duration,
                        Access = lesson.Access,
                        ThumbnailUrl = lesson.ThumbnailUrl,
                        VideoUrl = lesson.VideoUrl,
                        TranscriptUrl = lesson.TranscriptUrl,
                        Links = lesson.Links
                    };
                    return Results.Ok(pageDto);
                },
                CustomResults.Problem);
        })
        .WithMetadata<LessonDetailsPageDto>(
            nameof(GetLessonById),
            tag: Tags.Lessons,
            summary: "Gets a lesson by its ID.");
    }
}
