using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Common.Auth.Extentions;

public static class AuthorizationExtensions
{
    public static RouteHandlerBuilder RequirePermission(
        this RouteHandlerBuilder builder,
        PermissionType permission)
    {
        return builder.RequireAuthorization(permission.ToString());
    }

    public static RouteGroupBuilder RequirePermission(
        this RouteGroupBuilder group,
        PermissionType permission)
    {
        return group.RequireAuthorization(permission.ToString());
    }
}
