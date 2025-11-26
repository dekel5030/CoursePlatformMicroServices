using Application.Abstractions.Messaging;
using Domain.Enrollments.Primitives;

namespace Application.Enrollments.Commands.DeleteEnrollment;

public sealed record DeleteEnrollmentCommand(EnrollmentId Id) : ICommand;
