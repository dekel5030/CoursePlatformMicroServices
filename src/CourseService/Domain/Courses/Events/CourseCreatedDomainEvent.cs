using Kernel.Messaging.Abstractions;

namespace Courses.Domain.Courses.Events;

public sealed record CourseCreatedDomainEvent(ICourseSnapshot Course) : IDomainEvent;
