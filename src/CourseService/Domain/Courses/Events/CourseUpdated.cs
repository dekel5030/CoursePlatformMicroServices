using Kernel.Messaging.Abstractions;

namespace Courses.Domain.Courses.Events;

public sealed record CourseUpdated(ICourseSnapshot Course) : IDomainEvent;
