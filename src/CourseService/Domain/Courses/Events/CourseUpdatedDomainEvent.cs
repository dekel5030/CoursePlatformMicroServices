using Kernel.Messaging.Abstractions;

namespace Courses.Domain.Courses.Events;

public sealed record CourseUpdatedDomainEvent(ICourseSnapshot Course) : IDomainEvent;
