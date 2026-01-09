using Kernel.Messaging.Abstractions;

namespace Courses.Domain.Courses.Events;

public record CourseDeleted(Course Course) : IDomainEvent;