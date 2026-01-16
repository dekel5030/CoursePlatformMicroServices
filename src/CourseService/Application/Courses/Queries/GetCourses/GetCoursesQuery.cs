using Courses.Application.Courses.Dtos;
using Courses.Application.Shared.Dtos;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Courses.Queries.GetCourses;

public record GetCoursesQuery(PagedQueryDto PagedQuery) : IQuery<CourseCollectionDto>;
