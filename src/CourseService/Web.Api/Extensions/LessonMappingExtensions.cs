using Courses.Api.Contracts.Lessons;
using Courses.Api.Infrastructure.LinkProvider;
using Courses.Application.Actions;
using Courses.Application.Lessons.Dtos;
using Courses.Domain.Courses.Primitives;

namespace Courses.Api.Extensions;

internal static class LessonMappingExtensions
{
    public static LessonSummaryResponse ToApiContract(
        this LessonSummaryDto dto,
        CoursePolicyContext courseContext,
        LinkProvider linkProvider)
    {
        var lessonContext = new LessonPolicyContext(dto.LessonId, dto.Status, dto.Access);

        return new LessonSummaryResponse(
            dto.LessonId.Value,
            dto.Title.Value,
            dto.Index,
            dto.Duration,
            dto.ThumbnailUrl?.ToString(),
            dto.Status.ToString(),
            dto.Access.ToString(),
            linkProvider.CreateLessonLinks(courseContext, lessonContext));
    }

    public static LessonDetailsResponse ToApiContract(
        this LessonDetailsDto dto,
        CourseId courseId,
        LinkProvider linkProvider)
    {
        LessonPolicyContext lessonContext = new(dto.LessonId, dto.Status, dto.Access);

        return new LessonDetailsResponse(
            courseId.Value,
            dto.LessonId.Value,
            dto.Title.Value,
            dto.Description.Value,
            dto.Index,
            dto.Duration,
            dto.ThumbnailUrl?.ToString(),
            dto.Access.ToString(),
            dto.Status.ToString(),
            dto.VideoUrl?.ToString(),
            linkProvider.CreateLessonLinks(courseContext: dto.CourseContext, lessonContext: lessonContext));
    }
}
