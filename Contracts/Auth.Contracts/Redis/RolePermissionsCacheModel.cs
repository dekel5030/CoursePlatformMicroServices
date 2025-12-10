namespace Auth.Contracts.Redis;

public sealed record RolePermissionsCacheModel(
    string RoleName,
    IReadOnlyCollection<string> Permissions);