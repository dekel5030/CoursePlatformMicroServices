using Domain.Users.Primitives;
using Kernel.Messaging.Abstractions;

namespace Users.Application.Users.Commands.CreateUser;

public sealed record CreateUserCommand(
    string Email,
    string? UserId,
    string? FirstName,
    string? LastName,
    PhoneNumber? PhoneNumber,
    DateTime? DateOfBirth) : ICommand<CreatedUserRespondDto>
{ }