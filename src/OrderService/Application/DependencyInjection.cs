using Application.Abstractions.Messaging;
using Application.Orders.Commands.SubmitOrder;
using Application.Orders.DomainEvents;
using Application.Orders.Queries.Dtos;
using Application.Orders.Queries.GetById;
using Application.Orders.Queries.GetOrders;
using Domain.Orders.Events;
using Domain.Orders.Primitives;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddQueryHandlers();
        services.AddCommandHandlers();
        services.AddDomainEventHandlers();
        //services.AddValidators();

        return services;
    }

    private static IServiceCollection AddQueryHandlers(this IServiceCollection services)
    {
        services.AddScoped<IQueryHandler<GetOrderByIdQuery, OrderReadDto>, GetOrderByIdQueryHandler>();
        services.AddScoped<IQueryHandler<GetOrdersQuery, PagedResponse<OrderReadDto>>, GetOrdersQueryHandler>();

        return services;
    }

    private static IServiceCollection AddCommandHandlers(this IServiceCollection services)
    {
        services.AddScoped<ICommandHandler<SubmitOrderCommand, OrderId>, SubmitOrderCommandHandler>();

        return services;
    }

    private static IServiceCollection AddDomainEventHandlers(this IServiceCollection services)
    {
        services.AddScoped<IDomainEventHandler<OrderSubmittedDomainEvent>, OrderSubmittedDomainEventHandler>();

        return services;
    }

    private static IServiceCollection AddValidators(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        return services;
    }
}
