using Kernel.Auth;
using Kernel.Auth.AuthTypes;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Users.Application.Abstractions.Context;

namespace Users.Infrastructure.Auth;

public class CurrentUserContext : ICurrentUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

    public CurrentUserContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public bool IsAuthenticated => User?.Identity?.IsAuthenticated ?? false;

    public Guid? UserId
    {
        get
        {
            if (User == null || !IsAuthenticated)
            {
                return null;
            }

            var idValue = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(idValue, out var userId) ? userId : null;
        }
    }

    public bool HasRole(RoleType role)
    {
        return User?.IsInRole(role.ToString().ToLowerInvariant()) ?? false;
    }

    public bool HasPermissionOnUsersResource(ActionType action, Guid resourceId)
    {
        if (User == null)
        {
            return false;
        }

        var resourceIdObj = ResourceId.Create(resourceId.ToString());

        return PermissionEvaluator.HasPermission(User, action, ResourceType.User, resourceIdObj);
    }
}
