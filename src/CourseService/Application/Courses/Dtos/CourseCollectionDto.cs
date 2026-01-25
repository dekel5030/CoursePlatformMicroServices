using Courses.Application.Services.LinkProvider.Abstractions;
using Courses.Application.Shared.Dtos;

namespace Courses.Application.Courses.Dtos;

public sealed record CourseCollectionDto : PaginatedCollectionDto<CourseSummaryDto>;
