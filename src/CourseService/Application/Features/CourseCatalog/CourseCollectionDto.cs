using Courses.Application.Features.Dtos;
using Courses.Application.Shared.Dtos;

namespace Courses.Application.Features.CourseCatalog;

public sealed record CourseCollectionDto : PaginatedCollectionDto<CourseSummaryWithAnalyticsDto>;
