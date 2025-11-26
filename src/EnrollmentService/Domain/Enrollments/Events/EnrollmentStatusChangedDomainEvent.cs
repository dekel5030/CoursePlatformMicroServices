using Domain.Enrollments.Primitives;
using SharedKernel;

namespace Domain.Enrollments.Events;

public sealed record EnrollmentStatusChangedDomainEvent(
    EnrollmentId EnrollmentId,
    EnrollmentStatus OldStatus,
    EnrollmentStatus NewStatus) : IDomainEvent;
