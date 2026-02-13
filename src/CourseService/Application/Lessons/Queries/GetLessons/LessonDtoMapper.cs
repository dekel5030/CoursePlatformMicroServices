using Courses.Application.Lessons.Dtos;
using Courses.Domain.Lessons;

namespace Courses.Application.Lessons.Queries.GetLessons;

internal static class LessonDtoMapper
{
    internal static LessonDto Map(Lesson lesson, Func<string?, string> urlResolver)
    {
        var dto = new LessonDto
        {
            Id = lesson.Id.Value,
            Title = lesson.Title.Value,
            Index = lesson.Index,
            Duration = lesson.Duration,
            Access = lesson.Access,
            ThumbnailUrl = urlResolver(lesson.ThumbnailImageUrl?.Path),
            ModuleId = lesson.ModuleId.Value,
            CourseId = lesson.CourseId.Value,
            Description = lesson.Description.Value,
            VideoUrl = urlResolver(lesson.VideoUrl?.Path),
            TranscriptUrl = urlResolver(lesson.Transcript?.Path),
            Links = []
        };

        return dto;
    }
}
