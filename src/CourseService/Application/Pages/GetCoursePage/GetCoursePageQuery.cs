using Courses.Application.Courses.Dtos;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Pages.GetCoursePage;

public sealed record GetCoursePageQuery(Guid Id) : IQuery<CoursePageDto>;
