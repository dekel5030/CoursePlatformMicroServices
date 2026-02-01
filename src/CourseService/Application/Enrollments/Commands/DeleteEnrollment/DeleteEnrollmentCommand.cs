using Courses.Domain.Enrollments.Primitives;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Enrollments.Commands.DeleteEnrollment;

public sealed record DeleteEnrollmentCommand(EnrollmentId Id) : ICommand;
