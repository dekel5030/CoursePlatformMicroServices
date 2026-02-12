using Kernel.Messaging.Abstractions;

namespace Courses.Application.Pages.ManagedCoursePage;

public sealed record ManagedCoursePageQuery(Guid Id) : IQuery<ManagedCoursePageDto>;
