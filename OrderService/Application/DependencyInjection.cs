using Application.Abstractions.Messaging;
using Application.Orders.Commands.SubmitOrder;
using Application.Orders.DomainEvents;
using Application.Orders.Queries.Dtos;
using Application.Orders.Queries.GetById;
using Domain.Orders.Events;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddQueryHandlers();
        services.AddCommandHandlers();
        services.AddDomainEventHandlers();

        return services;
    }

    private static IServiceCollection AddQueryHandlers(this IServiceCollection services)
    {
        services.AddScoped<IQueryHandler<GetOrderByIdQuery, OrderReadDto>, GetOrderByIdHandler>();

        return services;
    }

    private static IServiceCollection AddCommandHandlers(this IServiceCollection services)
    {
        services.AddScoped<ICommandHandler<SubmitOrderCommand, Guid>, SubmitOrderCommandHandler>();

        return services;
    }

    private static IServiceCollection AddDomainEventHandlers(this IServiceCollection services)
    {
        services.AddScoped<IDomainEventHandler<OrderSubmitted>, OrderSubmittedHandler>();

        return services;
    }
}
