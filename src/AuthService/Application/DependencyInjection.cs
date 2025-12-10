using Application.Abstractions.Messaging;
using Application.AuthUsers.Commands.LoginUser;
using Application.AuthUsers.Commands.Logout;
using Application.AuthUsers.Commands.RegisterUser;
using Application.AuthUsers.Commands.UserAddPermission;
using Application.AuthUsers.Commands.UserAddRole;
using Application.AuthUsers.Commands.UserRemovePermission;
using Application.AuthUsers.Commands.UserRemoveRole;
using Application.AuthUsers.Dtos;
using Application.AuthUsers.Queries.GetCurrentUser;
using Application.Roles.Commands.CreateRole;
using Application.Roles.Commands.RoleAddPermission;
using Application.Roles.Commands.RoleRemovePermission;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddQueryHandlers();
        services.AddCommandHandlers();
        services.AddIntegrationEventHandlers();
        //services.AddValidators();

        return services;
    }

    private static IServiceCollection AddQueryHandlers(this IServiceCollection services)
    {
        services.AddScoped<IQueryHandler<GetCurrentUserQuery, CurrentUserDto>, GetCurrentUserQueryHandler>();
        return services;
    }

    private static IServiceCollection AddCommandHandlers(this IServiceCollection services)
    {
        services.AddScoped<ICommandHandler<RegisterUserCommand, CurrentUserDto>, RegisterUserCommandHandler>();
        services.AddScoped<ICommandHandler<LoginUserCommand, CurrentUserDto>, LoginUserCommandHandler>();
        services.AddScoped<ICommandHandler<LogoutCommand>, LogoutCommandHandler>();
        services.AddScoped<ICommandHandler<CreateRoleCommand, CreateRoleResponseDto>, CreateRoleCommandHandler>();
        services.AddScoped<ICommandHandler<RoleAddPermissionCommand>, RoleAddPermissionCommandHandler>();
        services.AddScoped<ICommandHandler<RoleRemovePermissionCommand>, RoleRemovePermissionCommandHandler>();
        
        services.AddScoped<ICommandHandler<UserAddRoleCommand>, UserAddRoleCommandHandler>();
        services.AddScoped<ICommandHandler<UserRemoveRoleCommand>, UserRemoveRoleCommandHandler>();
        services.AddScoped<ICommandHandler<UserAddPermissionCommand>, UserAddPermissionCommandHandler>();
        services.AddScoped<ICommandHandler<UserRemovePermissionCommand>, UserRemovePermissionCommandHandler>();
        return services;
    }

    private static IServiceCollection AddIntegrationEventHandlers(this IServiceCollection services)
    {
        // Integration event handlers will be added here
        return services;
    }

    private static IServiceCollection AddValidators(this IServiceCollection services)
    {
        // Validators can be added here
        return services;
    }
}
