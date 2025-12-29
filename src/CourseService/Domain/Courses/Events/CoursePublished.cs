using Kernel.Messaging.Abstractions;

namespace Courses.Domain.Courses.Events;

public record CoursePublished(Course Course) : IDomainEvent;