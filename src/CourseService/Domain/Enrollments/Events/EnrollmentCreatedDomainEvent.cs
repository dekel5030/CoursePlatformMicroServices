using Kernel.Messaging.Abstractions;

namespace Courses.Domain.Enrollments.Events;

public sealed record EnrollmentCreatedDomainEvent(IEnrollmentSnapshot Enrollment) : IDomainEvent;
