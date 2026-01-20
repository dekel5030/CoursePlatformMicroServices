using Courses.Domain.Courses.Primitives;
using Kernel.Messaging.Abstractions;

namespace Courses.Domain.Courses.Events;

public record StudentEnrolled(Course Course, UserId StudentId) : IDomainEvent;