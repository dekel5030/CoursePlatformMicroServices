using Application.Abstractions.Messaging;
using Application.Users.Commands.CreateUser;
using Application.Users.DomainEvents;
using Application.Users.IntegrationEvents.AuthRegistered;
using Application.Users.Queries.Dtos;
using Application.Users.Queries.GetUserByid;
using Application.Users.Queries.GetUsers;
using Domain.Users.Events;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IQueryHandler<GetUserByIdQuery, UserReadDto>, GetUserByIdQueryHandler>();
        services.AddScoped<IQueryHandler<GetUsersQuery, CollectionDto<UserReadDto>>, GetUsersQueryHandler>();

        services.AddScoped<ICommandHandler<CreateUserCommand, CreatedUserRespondDto>, CreateUserCommandHandler>();

        services.AddScoped<IIntegrationEventHandler<AuthRegisteredIntegrationEvent>, AuthRegisteredIntegrationEventHandler>();
        
        services.AddScoped<IDomainEventHandler<UserProfileCreatedDomainEvent>, UserProfileCreatedDomainEventHandler>();
        
        return services;
    }
}
