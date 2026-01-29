using Courses.Application.Courses.Dtos;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Courses.Queries.GetCoursePage;

internal sealed record GetCoursePageQuery(Guid Id) : IQuery<CoursePageDto>;
