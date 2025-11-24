using Application.Abstractions.Messaging;

namespace Application.Enrollments.Commands.DeleteEnrollment;

public sealed record DeleteEnrollmentCommand(int EnrollmentId) : ICommand;
