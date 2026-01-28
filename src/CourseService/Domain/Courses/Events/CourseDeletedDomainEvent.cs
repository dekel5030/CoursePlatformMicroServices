using Kernel.Messaging.Abstractions;

namespace Courses.Domain.Courses.Events;

public sealed record CourseDeletedDomainEvent(ICourseSnapshot Course) : IDomainEvent;
