using Kernel.Messaging.Abstractions;

namespace Courses.Domain.Enrollments.Events;

public sealed record EnrollmentStatusChangedDomainEvent(IEnrollmentSnapshot Enrollment) : IDomainEvent;
