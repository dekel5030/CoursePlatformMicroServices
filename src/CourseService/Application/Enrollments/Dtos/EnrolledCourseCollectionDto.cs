using Courses.Application.Shared.Dtos;

namespace Courses.Application.Enrollments.Dtos;

public sealed record EnrolledCourseCollectionDto : PaginatedCollectionDto<EnrolledCourseDto>;
