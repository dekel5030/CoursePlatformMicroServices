using Courses.Application.Courses.Dtos;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Courses.Queries.GetManagedCourses;

public sealed record GetManagedCoursesQuery(
    int PageNumber = 1,
    int PageSize = 10) : IQuery<CourseCollectionDto>;
