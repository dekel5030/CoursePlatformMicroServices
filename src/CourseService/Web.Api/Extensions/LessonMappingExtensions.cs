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
        var lessonContext = new LessonPolicyContext(dto.LessonId, dto.Access);

        return new LessonSummaryResponse(
            dto.LessonId.Value,
            dto.Title.Value,
            dto.Index,
            dto.Duration,
            dto.ThumbnailUrl?.ToString(),
            "Draft", // LessonSummaryDto doesn't have Status, using default
            dto.Access.ToString(),
            linkProvider.CreateLessonLinks(courseContext, lessonContext));
    }

    public static LessonDetailsResponse ToApiContract(
        this LessonDetailsDto dto,
        CourseId courseId,
        LinkProvider linkProvider)
    {
        LessonPolicyContext lessonContext = new(dto.LessonId, dto.Access);

        // Create a minimal course context for links
        var courseContext = new CoursePolicyContext(
            courseId,
            new Domain.Courses.Primitives.UserId(Guid.Empty),
            Domain.Courses.Primitives.CourseStatus.Draft,
            0);

        return new LessonDetailsResponse(
            courseId.Value,
            dto.LessonId.Value,
            dto.Title.Value,
            dto.Description.Value,
            dto.Index,
            dto.Duration,
            dto.ThumbnailUrl?.ToString(),
            dto.Access.ToString(),
            "Draft", // LessonDetailsDto doesn't have Status, using default
            dto.VideoUrl?.ToString(),
            linkProvider.CreateLessonLinks(courseContext: courseContext, lessonContext: lessonContext));
    }
}
