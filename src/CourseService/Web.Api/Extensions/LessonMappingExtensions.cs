using Courses.Api.Contracts.Lessons;
using Courses.Application.Lessons.Queries.Dtos;

namespace Courses.Api.Extensions;

internal static class LessonMappingExtensions
{
    public static LessonSummaryResponse ToApiContract(this LessonSummaryDto dto)
    {
        return new LessonSummaryResponse(
            dto.LessonId.Value,
            dto.Title.Value,
            dto.Description.Value,
            dto.Index,
            dto.Duration,
            dto.IsPreview,
            dto.ThumbnailUrl?.ToString());
    }

    public static LessonDetailsResponse ToApiContract(this LessonDetailsDto dto)
    {
        return new LessonDetailsResponse(
            dto.CourseId.Value,
            dto.LessonId.Value,
            dto.Title.Value,
            dto.Description.Value,
            dto.Index,
            dto.Duration,
            dto.IsPreview,
            dto.ThumbnailUrl?.ToString(),
            dto.VideoUrl?.ToString());
    }
}
