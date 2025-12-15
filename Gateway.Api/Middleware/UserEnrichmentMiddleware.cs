using System.Security.Claims;
using CoursePlatform.ServiceDefaults.Auth;
using Gateway.Api.Services.UserPermissionsService;

namespace Gateway.Api.Middleware;

public class UserEnrichmentMiddleware : IMiddleware
{
    private readonly IUserPermissionsService _userPermissionsService;

    public UserEnrichmentMiddleware(
        IUserPermissionsService userPermissionsService)
    {
        _userPermissionsService = userPermissionsService;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (context.User.Identity?.IsAuthenticated != true)
        {
            await next(context);
            return;
        }

        var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            await next(context);
            return;
        }

        var permissionsDto = await _userPermissionsService
            .GetUserPermissionsAsync(userId, context.RequestAborted);

        context.Request.Headers.Append(HeaderNames.UserId, userId);

        if (permissionsDto.Permissions != null && permissionsDto.Permissions.Count > 0)
        {
            var permissionsHeaderValue = string.Join(",", permissionsDto.Permissions);

            context.Request.Headers.Append(HeaderNames.UserPermissions, permissionsHeaderValue);
        }

        if (permissionsDto.Roles != null && permissionsDto.Roles.Count > 0)
        {
            var rolesHeaderValue = string.Join(",", permissionsDto.Roles);

            context.Request.Headers.Append(HeaderNames.UserRoles, rolesHeaderValue);
        }

        await next(context);
    }
}
