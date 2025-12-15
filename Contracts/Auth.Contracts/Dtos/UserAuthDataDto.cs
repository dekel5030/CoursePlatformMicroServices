namespace Auth.Contracts.Dtos;

public record UserAuthDataDto(
    string UserId,
    List<string> Permissions,
    List<string> Roles);