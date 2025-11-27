using Application.Abstractions.Messaging;
using Application.Admin.Commands.AssignPermissionToRole;
using Application.Admin.Commands.AssignPermissionToUser;
using Application.Admin.Commands.AssignRoleToUser;
using Application.Admin.Commands.CreatePermission;
using Application.Admin.Commands.CreateRole;
using Application.Admin.Commands.DeletePermission;
using Application.Admin.Commands.DeleteRole;
using Application.Admin.Commands.RemovePermissionFromRole;
using Application.Admin.Commands.RemovePermissionFromUser;
using Application.Admin.Commands.RemoveRoleFromUser;
using Application.Admin.Dtos;
using Application.Admin.Queries.GetAllPermissions;
using Application.Admin.Queries.GetAllRoles;
using Application.Admin.Queries.GetAllUsers;
using Application.Admin.Queries.GetRolePermissions;
using Application.Admin.Queries.GetUserPermissions;
using Application.AuthUsers.Commands.LoginUser;
using Application.AuthUsers.Commands.Logout;
using Application.AuthUsers.Commands.RefreshAccessToken;
using Application.AuthUsers.Commands.RegisterUser;
using Application.AuthUsers.Dtos;
using Application.AuthUsers.Events;
using Application.AuthUsers.Queries.GetCurrentUser;
using Domain.AuthUsers.Events;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddQueryHandlers();
        services.AddCommandHandlers();
        services.AddDomainEventHandlers();
        services.AddIntegrationEventHandlers();
        //services.AddValidators();

        return services;
    }

    private static IServiceCollection AddQueryHandlers(this IServiceCollection services)
    {
        services.AddScoped<IQueryHandler<GetCurrentUserQuery, AuthResponseDto>, GetCurrentUserQueryHandler>();
        services.AddScoped<IQueryHandler<GetAllPermissionsQuery, IEnumerable<PermissionDto>>, GetAllPermissionsQueryHandler>();
        services.AddScoped<IQueryHandler<GetAllRolesQuery, IEnumerable<RoleDto>>, GetAllRolesQueryHandler>();
        services.AddScoped<IQueryHandler<GetAllUsersQuery, IEnumerable<UserDto>>, GetAllUsersQueryHandler>();
        services.AddScoped<IQueryHandler<GetRolePermissionsQuery, IEnumerable<PermissionDto>>, GetRolePermissionsQueryHandler>();
        services.AddScoped<IQueryHandler<GetUserPermissionsQuery, IEnumerable<PermissionDto>>, GetUserPermissionsQueryHandler>();
        return services;
    }

    private static IServiceCollection AddCommandHandlers(this IServiceCollection services)
    {
        services.AddScoped<ICommandHandler<RegisterUserCommand, AuthTokensResult>, RegisterUserCommandHandler>();
        services.AddScoped<ICommandHandler<LoginUserCommand, AuthTokensResult>, LoginUserCommandHandler>();
        services.AddScoped<ICommandHandler<RefreshAccessTokenCommand, string>, RefreshAccessTokenCommandHandler>();
        services.AddScoped<ICommandHandler<LogoutCommand>, LogoutCommandHandler>();

        services.AddScoped<ICommandHandler<CreatePermissionCommand, PermissionDto>, CreatePermissionCommandHandler>();
        services.AddScoped<ICommandHandler<DeletePermissionCommand>, DeletePermissionCommandHandler>();
        services.AddScoped<ICommandHandler<CreateRoleCommand, RoleDto>, CreateRoleCommandHandler>();
        services.AddScoped<ICommandHandler<DeleteRoleCommand>, DeleteRoleCommandHandler>();
        services.AddScoped<ICommandHandler<AssignPermissionToRoleCommand>, AssignPermissionToRoleCommandHandler>();
        services.AddScoped<ICommandHandler<RemovePermissionFromRoleCommand>, RemovePermissionFromRoleCommandHandler>();
        services.AddScoped<ICommandHandler<AssignPermissionToUserCommand>, AssignPermissionToUserCommandHandler>();
        services.AddScoped<ICommandHandler<RemovePermissionFromUserCommand>, RemovePermissionFromUserCommandHandler>();
        services.AddScoped<ICommandHandler<AssignRoleToUserCommand>, AssignRoleToUserCommandHandler>();
        services.AddScoped<ICommandHandler<RemoveRoleFromUserCommand>, RemoveRoleFromUserCommandHandler>();

        return services;
    }

    private static IServiceCollection AddDomainEventHandlers(this IServiceCollection services)
    {
        services.AddScoped<IDomainEventHandler<UserRegisteredDomainEvent>, UserRegisteredDomainEventHandler>();

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
