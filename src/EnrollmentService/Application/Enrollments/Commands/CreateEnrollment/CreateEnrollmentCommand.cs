using Application.Abstractions.Messaging;
using Domain.Enrollments.Primitives;

namespace Application.Enrollments.Commands.CreateEnrollment;

public sealed record CreateEnrollmentCommand(
    CreateEnrollmentDto Dto) : ICommand<EnrollmentId>;
