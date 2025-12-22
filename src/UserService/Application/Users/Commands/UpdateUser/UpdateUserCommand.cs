using Domain.Users.Primitives;
using Kernel.Messaging.Abstractions;

namespace Users.Application.Users.Commands.UpdateUser;

public sealed record UpdateUserCommand(
    Guid UserId,
    string? FirstName,
    string? LastName,
    PhoneNumber? PhoneNumber,
    DateTime? DateOfBirth) : ICommand<UpdatedUserResponseDto>
{ }