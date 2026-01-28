using Courses.Domain.Courses;
using Courses.Domain.Courses.Primitives;
using Kernel.Messaging.Abstractions;

namespace Courses.Domain.Enrollments.Events;

public record StudentEnrolled(Course Course, UserId StudentId) : IDomainEvent;
