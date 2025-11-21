using Application.Abstractions.Messaging;
using Domain.Users.Primitives;

namespace Application.Users.Commands.CreateUser;

public sealed record CreateUserCommand(
    string Email,
    string? UserId, // Optional - if provided, must match AuthUserId
    string? FirstName,
    string? LastName,
    PhoneNumber? PhoneNumber, 
    DateTime? DateOfBirth) : ICommand<CreatedUserRespondDto>
{ }