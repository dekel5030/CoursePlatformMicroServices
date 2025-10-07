namespace Application.Users.Queries.GetUserByid;

public record UserReadDto(
    Guid Id,
    string Email,
    string? FirstName,
    string? LastName,
    DateTime? DateOfBirth,
    string? PhoneNumber);