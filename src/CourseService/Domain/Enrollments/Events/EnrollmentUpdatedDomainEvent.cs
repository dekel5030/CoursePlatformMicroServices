using Kernel.Messaging.Abstractions;

namespace Courses.Domain.Enrollments.Events;

public sealed record EnrollmentUpdatedDomainEvent(IEnrollmentSnapshot Enrollment) : IDomainEvent;
