using Kernel.Messaging.Abstractions;

namespace Courses.Application.Features.CoursePage;

public sealed record CoursePageQuery(Guid Id) : IQuery<CoursePageDto>;
