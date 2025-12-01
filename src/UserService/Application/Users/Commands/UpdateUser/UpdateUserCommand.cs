using Application.Abstractions.Messaging;
using Domain.Users.Primitives;

namespace Application.Users.Commands.UpdateUser;

public sealed record UpdateUserCommand(
    Guid UserId,
    string? FirstName,
    string? LastName,
    PhoneNumber? PhoneNumber,
    DateTime? DateOfBirth) : ICommand<UpdatedUserResponseDto>
{ }