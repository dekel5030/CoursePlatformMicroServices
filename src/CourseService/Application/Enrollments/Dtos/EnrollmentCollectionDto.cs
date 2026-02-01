using Courses.Application.Shared.Dtos;

namespace Courses.Application.Enrollments.Dtos;

public sealed record EnrollmentCollectionDto : PaginatedCollectionDto<EnrollmentDto>;
