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

    public string? UserEmail => User?.FindFirst(ClaimTypes.Email)?.Value;

    public string? FirstName => User?.FindFirst(ClaimTypes.GivenName)?.Value;

    public string? LastName => User?.FindFirst(ClaimTypes.Surname)?.Value;

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
