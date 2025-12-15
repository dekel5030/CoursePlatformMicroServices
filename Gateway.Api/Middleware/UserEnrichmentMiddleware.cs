using System.Security.Claims;
using CoursePlatform.ServiceDefaults.Auth;
using Gateway.Api.Models;
using Gateway.Api.Services.UserEnrichmentService;
    
namespace Gateway.Api.Middleware;

public class UserEnrichmentMiddleware : IMiddleware
{
    private readonly IUserEnrichmentService _userEnrichmentService;

    public UserEnrichmentMiddleware(
        IUserEnrichmentService userPermissionsService)
    {
        _userEnrichmentService = userPermissionsService;
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

        UserEnrichmentModel userModel = await _userEnrichmentService
            .GetUserPermissionsAsync(userId, context.RequestAborted);

        context.Request.Headers[HeaderNames.UserId] = userId;

        if (userModel.Permissions != null && userModel.Permissions.Count > 0)
        {
            var permissionsHeaderValue = string.Join(",", userModel.Permissions);

            context.Request.Headers[HeaderNames.UserPermissions] = permissionsHeaderValue;
        }
        else
        {
            context.Request.Headers.Remove(HeaderNames.UserPermissions);
        }

        if (userModel.Roles != null && userModel.Roles.Count > 0)
        {
            var rolesHeaderValue = string.Join(",", userModel.Roles);
            context.Request.Headers[HeaderNames.UserRoles] = rolesHeaderValue;
        }
        else
        {
            context.Request.Headers.Remove(HeaderNames.UserRoles);
        }

        await next(context);
    }
}
