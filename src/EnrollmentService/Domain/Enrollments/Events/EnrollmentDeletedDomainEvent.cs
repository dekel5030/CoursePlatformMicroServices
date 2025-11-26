using Domain.Enrollments.Primitives;
using SharedKernel;

namespace Domain.Enrollments.Events;

public sealed record EnrollmentDeletedDomainEvent(
    EnrollmentId EnrollmentId) : IDomainEvent;
