using System.Security.Claims;
using Kernel.Auth;
using Kernel.Auth.Abstractions;
using Kernel.Auth.AuthTypes;
using Microsoft.AspNetCore.Http;

namespace CoursePlatform.ServiceDefaults.Auth;

public class UserContext : IUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

    public bool IsAuthenticated => User?.Identity?.IsAuthenticated ?? false;

    public Guid? UserId
    {
        get
        {
            if (User == null || !IsAuthenticated) 
            { 
                return null; 
            }

            string? idValue = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(idValue, out var userId) ? userId : null;
        }
    }

    public bool HasRole(RoleType role)
    {
        return User?.IsInRole(role.ToString()) ?? false;
    }

    public bool HasPermission(ActionType action, ResourceType resource, ResourceId resourceId)
    {
        if (User == null) return false;

        return PermissionEvaluator.HasPermission(User, action, resource, resourceId);
    }
}
