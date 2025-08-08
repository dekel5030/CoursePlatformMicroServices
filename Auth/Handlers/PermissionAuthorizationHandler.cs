using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Auth.Handlers;

public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    private IServiceScopeFactory _serviceScopeFactory;

    public PermissionAuthorizationHandler(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        var permission = context.User.Claims
        .FirstOrDefault(x => x.Type == CustomClaimNames.Permission && x.Value == requirement.Permission)?.Value;

        if (permission != null)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}