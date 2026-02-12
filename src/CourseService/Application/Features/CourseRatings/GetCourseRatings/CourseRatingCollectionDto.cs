using Courses.Application.Shared.Dtos;

namespace Courses.Application.Features.CourseRatings.GetCourseRatings;

public sealed record CourseRatingCollectionDto : PaginatedCollectionDto<CourseRatingDto>;
