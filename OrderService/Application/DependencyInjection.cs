using Application.Abstractions.Messaging;
using Application.Orders.DomainEvents;
using Application.Orders.Queries.Dtos;
using Application.Orders.Queries.GetById;
using Domain.Orders.Events;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddQueryHandlers();
        services.AddDomainEventHandlers();

        return services;
    }

    private static IServiceCollection AddQueryHandlers(this IServiceCollection services)
    {
        services.AddScoped<IQueryHandler<GetOrderByIdQuery, OrderReadDto>, GetOrderByIdHandler>();

        return services;
    }

    private static IServiceCollection AddDomainEventHandlers(this IServiceCollection services)
    {
        services.AddScoped<IDomainEventHandler<OrderCreated>, OrderCreatedHandler>();

        return services;
    }
}
