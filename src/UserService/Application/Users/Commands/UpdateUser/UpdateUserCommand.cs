using Kernel.Messaging.Abstractions;
using Users.Domain.Users.Primitives;

namespace Users.Application.Users.Commands.UpdateUser;

public sealed record UpdateUserCommand(
    Guid UserId,
    string? FirstName,
    string? LastName,
    PhoneNumber? PhoneNumber,
    DateTime? DateOfBirth) : ICommand<UpdatedUserResponseDto>
{ }