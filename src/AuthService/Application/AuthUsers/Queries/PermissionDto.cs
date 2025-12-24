namespace Auth.Application.AuthUsers.Queries;

public record PermissionDto(
    string Key,
    string Effect,
    string Action,
    string Resource,
    string ResourceId
);