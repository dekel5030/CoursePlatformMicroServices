using Kernel.Messaging.Abstractions;

namespace Courses.Domain.Courses.Events;

public sealed record CourseCreated(ICourseSnapshot Course) : IDomainEvent;
