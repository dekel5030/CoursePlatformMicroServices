using Application.Abstractions.Messaging;
using Application.AuthUsers.Commands.AddRole;
using Application.AuthUsers.Commands.LoginUser;
using Application.AuthUsers.Commands.Logout;
using Application.AuthUsers.Commands.RegisterUser;
using Application.AuthUsers.Dtos;
using Application.AuthUsers.Queries.GetCurrentUser;
using Application.Roles.Commands.AddRolePermission;
using Application.Roles.Commands.CreateRole;
using Application.Roles.Commands.RemoveRolePermission;
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
        services.AddScoped<ICommandHandler<AddRolePermissionCommand>, AddRolePermissionCommandHandler>();
        services.AddScoped<ICommandHandler<RemoveRolePermissionCommand>, RemoveRolePermissionCommandHandler>();
        services.AddScoped<ICommandHandler<UserAddRoleCommand>, UserAddRoleCommandHandler>();
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
