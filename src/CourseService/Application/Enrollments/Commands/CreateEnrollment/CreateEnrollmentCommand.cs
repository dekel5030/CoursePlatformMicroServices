using Courses.Domain.Courses.Primitives;
using Courses.Domain.Enrollments.Primitives;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Enrollments.Commands.CreateEnrollment;

public sealed record CreateEnrollmentCommand(
    CourseId CourseId,
    UserId StudentId,
    DateTimeOffset? EnrolledAt,
    DateTimeOffset? ExpiresAt) : ICommand<CreateEnrollmentResponse>;
