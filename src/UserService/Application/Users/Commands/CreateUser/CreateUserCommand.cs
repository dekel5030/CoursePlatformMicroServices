using Kernel.Messaging.Abstractions;
using Users.Domain.Users.Primitives;

namespace Users.Application.Users.Commands.CreateUser;

public sealed record CreateUserCommand(
    string Email,
    string? UserId,
    string? FirstName,
    string? LastName,
    PhoneNumber? PhoneNumber,
    DateTime? DateOfBirth) : ICommand<CreatedUserRespondDto>
{ }