namespace Auth.Contracts.Redis.Events;

public sealed record RolePermissionsChangedEvent(
    string RoleName);