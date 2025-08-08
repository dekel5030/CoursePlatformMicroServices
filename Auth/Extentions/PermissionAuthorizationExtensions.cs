using Common.Auth.Handlers;
using Common.Auth.Providers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Auth.Extentions;

public static class PermissionAuthorizationExtensions
{
    public static IServiceCollection AddPermissionAuthorization(this IServiceCollection services)
    {
        services.AddAuthorization();
        services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();
        services.AddSingleton<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();

        return services;
    }
}