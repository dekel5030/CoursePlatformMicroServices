using Application.Courses.Queries.Dtos;
using Kernel.Messaging.Abstractions;

namespace Application.Courses.Queries.GetCourses;

public record GetCoursesQuery(PagedQueryDto PagedQuery) : IQuery<PagedResponseDto<CourseReadDto>>;
