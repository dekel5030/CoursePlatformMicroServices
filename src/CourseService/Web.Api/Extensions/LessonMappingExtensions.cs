using Courses.Api.Contracts.Lessons;
using Courses.Api.Infrastructure.LinkProvider;
using Courses.Application.Lessons.Queries.Dtos;
using Courses.Domain.Courses.Primitives;

namespace Courses.Api.Extensions;

internal static class LessonMappingExtensions
{
    public static LessonSummaryResponse ToApiContract(
        this LessonSummaryDto dto,
        CourseId courseId,
        LinkProvider linkProvider)
    {
        return new LessonSummaryResponse(
            dto.LessonId.Value,
            dto.Title.Value,
            dto.Description.Value,
            dto.Index,
            dto.Duration,
            dto.IsPreview,
            dto.ThumbnailUrl?.ToString(),
            linkProvider.CreateLessonLinks(courseId, dto.LessonId, dto.AllowedActions));
    }

    public static LessonDetailsResponse ToApiContract(
        this LessonDetailsDto dto,
        CourseId courseId,
        LinkProvider linkProvider)
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
            dto.VideoUrl?.ToString(),
            linkProvider.CreateLessonLinks(courseId, dto.LessonId, new List<LessonAction>()));
    }
}
