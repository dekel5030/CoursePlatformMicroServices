using Application.Abstractions.Messaging;
using Domain.Enrollments.Primitives;

namespace Application.Enrollments.Commands.CreateEnrollment;

public sealed record CreateEnrollmentCommand(
    int UserId,
    int CourseId,
    DateTime? ExpiresAt = null) : ICommand<EnrollmentId>;
