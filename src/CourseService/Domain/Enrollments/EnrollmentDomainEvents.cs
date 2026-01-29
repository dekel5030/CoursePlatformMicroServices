using Courses.Domain.Courses.Primitives;
using Courses.Domain.Enrollments.Primitives;
using Courses.Domain.Lessons.Primitives;
using Kernel.Messaging.Abstractions;

namespace Courses.Domain.Enrollments;

public record EnrollmentCreatedDomainEvent(
    EnrollmentId Id,
    CourseId CourseId,
    UserId StudentId,
    DateTimeOffset EnrolledAt) : IDomainEvent;

public record LessonCompletedDomainEvent(
    EnrollmentId Id,
    CourseId CourseId,
    UserId StudentId,
    LessonId LessonId,
    bool CourseFullyCompleted) : IDomainEvent;

public record EnrollmentStatusChangedDomainEvent(
    EnrollmentId Id,
    CourseId CourseId,
    EnrollmentStatus NewStatus) : IDomainEvent;