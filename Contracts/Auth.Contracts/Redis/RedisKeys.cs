namespace Auth.Contracts.Redis;
public static class RedisKeys
{
    public static string RolePermissions(string roleName)
        => $"auth:roles:{roleName.ToLowerInvariant()}";
}
