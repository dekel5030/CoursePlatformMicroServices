namespace Users.Application.Users.Commands.UpdateUser;

public sealed record UpdatedUserResponseDto(
    Guid Id,
    string Email,
    string? FirstName,
    string? LastName,
    DateTime? DateOfBirth,
    string? PhoneNumber);