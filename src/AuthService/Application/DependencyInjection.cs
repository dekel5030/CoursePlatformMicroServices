using Application.Abstractions.Messaging;
using Application.AuthUsers.Commands.LoginUser;
using Application.AuthUsers.Commands.RegisterUser;
using Application.AuthUsers.Dtos;
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
        // Query handlers will be added here
        return services;
    }

    private static IServiceCollection AddCommandHandlers(this IServiceCollection services)
    {
        services.AddScoped<ICommandHandler<RegisterUserCommand, AuthResponseDto>, RegisterUserCommandHandler>();
        services.AddScoped<ICommandHandler<LoginUserCommand, AuthResponseDto>, LoginUserCommandHandler>();

        return services;
    }

    private static IServiceCollection AddDomainEventHandlers(this IServiceCollection services)
    {
        // Domain event handlers will be added here
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
