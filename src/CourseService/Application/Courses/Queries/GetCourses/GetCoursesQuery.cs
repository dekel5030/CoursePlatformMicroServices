using Application.Abstractions.Messaging;
using Application.Courses.Queries.Dtos;

namespace Application.Courses.Queries.GetCourses;

public record GetCoursesQuery(PagedQueryDto PagedQuery) : IQuery<PagedResponseDto<CourseReadDto>>;
