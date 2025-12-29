using Kernel.Messaging.Abstractions;

namespace Courses.Domain.Courses.Events;

public record StudentEnrolled(Course Course, StudentId StudentId) : IDomainEvent;