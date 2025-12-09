using Application.Abstractions.Messaging;
using Domain.AuthUsers.Events;
using Infrastructure.DomainEvents.IntegrationEvents;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.DomainEvents;

internal static class DomainEventsExtensions
{
    public static IServiceCollection AddDomainEventsHandlers(this IServiceCollection services)
    {
        return services.AddScoped<IDomainEventHandler<UserRegisteredDomainEvent>, UserRegisteredDomainEventHandler>();
    }
}
