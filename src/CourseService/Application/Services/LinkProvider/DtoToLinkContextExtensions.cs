using Courses.Application.Courses.Dtos;
using Courses.Application.Shared.Dtos;
using Courses.Domain.Courses.Primitives;
using Courses.Domain.Ratings.Primitives;

namespace Courses.Application.Services.LinkProvider;

internal static class DtoToLinkContextExtensions
{
    public static CourseContext ToCourseContext(this CourseSummaryDto dto)
    {
        return new CourseContext(
            new CourseId(dto.Id),
            new UserId(dto.Instructor.Id),
            dto.Status);
    }

    public static CourseContext ToCourseContext(this CourseSummaryWithAnalyticsDto dto)
    {
        return dto.Course.ToCourseContext();
    }

    public static CourseCollectionContext ToCourseCollectionContext(
        this PagedQueryDto query,
        int totalCount)
    {
        return new CourseCollectionContext(query, totalCount);
    }

    public static CourseRatingLinkContext ToRatingLinkContext(
        this CourseRatingDto dto,
        UserId? currentUserId)
    {
        return new CourseRatingLinkContext(
            new RatingId(dto.Id),
            new UserId(dto.User.Id),
            currentUserId);
    }
}
