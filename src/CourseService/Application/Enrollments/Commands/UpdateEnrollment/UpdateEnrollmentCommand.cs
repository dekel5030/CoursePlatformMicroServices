using Courses.Domain.Enrollments.Primitives;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Enrollments.Commands.UpdateEnrollment;

public sealed record UpdateEnrollmentCommand(
    EnrollmentId Id,
    DateTimeOffset? ExpiresAt,
    bool? Revoke) : ICommand;
