namespace Application.Users.Queries.GetUserByid;

public record UserReadDto(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    DateTime? DateOfBirth,
    string? PhoneNumber);