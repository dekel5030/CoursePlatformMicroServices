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

    public Guid? Id => Guid.TryParse(User?.FindFirst(CoursePlatformClaims.UserId)?.Value, out var id) ? id : null;

    public string? Email => User?.FindFirst(CoursePlatformClaims.Email)?.Value;

    public string? FirstName => User?.FindFirst(CoursePlatformClaims.FirstName)?.Value;

    public string? LastName => User?.FindFirst(CoursePlatformClaims.LastName)?.Value;

    public bool HasRole(RoleType role) => User?.IsInRole(role.ToString()) ?? false;

    public bool HasPermission(ActionType action, ResourceType resource, ResourceId resourceId)
    {
        if (User == null) 
        { 
            return false; 
        }

        return PermissionEvaluator.HasPermission(User, action, resource, resourceId);
    }
}
