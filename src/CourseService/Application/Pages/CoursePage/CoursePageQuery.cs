using Kernel.Messaging.Abstractions;

namespace Courses.Application.Pages.CoursePage;

public sealed record CoursePageQuery(Guid Id) : IQuery<CoursePageDto>;
