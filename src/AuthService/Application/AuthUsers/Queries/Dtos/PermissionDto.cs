namespace Auth.Application.AuthUsers.Queries.Dtos;

public record PermissionDto(
    string Key,
    string Effect,
    string Action,
    string Resource,
    string ResourceId
);