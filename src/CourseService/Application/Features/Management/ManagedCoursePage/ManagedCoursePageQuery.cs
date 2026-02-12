using Kernel.Messaging.Abstractions;

namespace Courses.Application.Features.Management.ManagedCoursePage;

public sealed record ManagedCoursePageQuery(Guid Id) : IQuery<ManagedCoursePageDto>;
