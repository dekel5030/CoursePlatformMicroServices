namespace Kernel.Auth;

public static class AuthCacheKeys
{
    public const string UserAuthDataPattern = "auth:permissions:{0}";
    public const string UserRolePermissionsPattern = "auth:roles:{0}";

    public static string UserInternalJwt(string identityUserId) => $"auth:{identityUserId}:internal-jwt";
}
