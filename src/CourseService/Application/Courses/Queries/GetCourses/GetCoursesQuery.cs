using Courses.Application.Courses.Queries.Dtos;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Courses.Queries.GetCourses;

public record GetCoursesQuery(PagedQueryDto PagedQuery) : IQuery<PagedResponseDto<CourseSummaryDto>>;
