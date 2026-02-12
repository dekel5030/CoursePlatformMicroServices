using Courses.Application.Shared.Dtos;

namespace Courses.Application.CourseRatings.Queries.GetCourseRatings;

public sealed record CourseRatingCollectionDto : PaginatedCollectionDto<CourseRatingDto>;
