using Domain.Enrollments.Primitives;
using Domain.Users.Primitives;
using Domain.Courses.Primitives;
using SharedKernel;

namespace Domain.Enrollments.Events;

public sealed record EnrollmentCreatedDomainEvent(
    EnrollmentId EnrollmentId,
    ExternalUserId UserId,
    ExternalCourseId CourseId,
    EnrollmentStatus Status,
    DateTime EnrolledAt) : IDomainEvent;
