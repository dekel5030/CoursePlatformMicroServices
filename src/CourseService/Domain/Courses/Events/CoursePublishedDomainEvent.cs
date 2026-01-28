using Kernel.Messaging.Abstractions;

namespace Courses.Domain.Courses.Events;

public record CoursePublishedDomainEvent(ICourseSnapshot Course) : IDomainEvent;
