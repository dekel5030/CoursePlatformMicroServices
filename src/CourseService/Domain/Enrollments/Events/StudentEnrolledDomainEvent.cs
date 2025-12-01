using Domain.Courses.Primitives;
using Domain.Enrollments.Primitives;
using SharedKernel;

namespace Domain.Enrollments.Events;

public sealed record StudentEnrolledDomainEvent(
    EnrollmentId EnrollmentId,
    Guid StudentId,
    CourseId CourseId,
    DateTime EnrollmentDate) : IDomainEvent;
