using CoursePlatform.ServiceDefaults.Messaging;
using Kernel.Messaging.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Users.Application.Users.Commands.CreateUser;
using Users.Application.Users.Commands.UpdateUser;
using Users.Application.Users.Queries.Dtos;
using Users.Application.Users.Queries.GetUserByid;
using Users.Application.Users.Queries.GetUsers;

namespace Users.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IQueryHandler<GetUserByIdQuery, UserReadDto>, GetUserByIdQueryHandler>();
        services.AddScoped<IQueryHandler<GetUsersQuery, CollectionDto<UserReadDto>>, GetUsersQueryHandler>();

        services.AddScoped<ICommandHandler<CreateUserCommand, CreatedUserRespondDto>, CreateUserCommandHandler>();
        services.AddScoped<ICommandHandler<UpdateUserCommand, UpdatedUserResponseDto>, UpdateUserCommandHandler>();


        services.AddScoped<IMediator, Mediator>();
        services.AddEventHandler();
        return services;
    }

    private static IServiceCollection AddEventHandler(this IServiceCollection services)
    {
        services.Scan(selector => selector
                .FromAssemblies(typeof(DependencyInjection).Assembly)
                .AddClasses(classes => classes
                    .Where(t => t.GetInterfaces().Any(i =>
                        i.IsGenericType &&
                        i.GetGenericTypeDefinition() == typeof(IEventHandler<>))),
                    publicOnly: false)
                .AsImplementedInterfaces()
                .WithScopedLifetime());

        return services;

    }
}