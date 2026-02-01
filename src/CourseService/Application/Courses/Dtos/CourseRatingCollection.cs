using Courses.Application.Shared.Dtos;

namespace Courses.Application.Courses.Dtos;

public sealed record CourseRatingCollection : PaginatedCollectionDto<CourseRatingDto>;
