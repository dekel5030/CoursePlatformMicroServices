using Application.Abstractions.Messaging;
using Application.Courses.Queries.Dtos;
using Application.Courses.Queries.GetById;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddQueryHandlers();
        services.AddCommandHandlers();
        services.AddDomainEventHandlers();
        services.AddIntegrataionEventHandlers();
        //services.AddValidators();

        return services;
    }

    private static IServiceCollection AddQueryHandlers(this IServiceCollection services)
    {
        services.AddScoped<IQueryHandler<GetCourseByIdQuery, CourseReadDto>, GetCourseByIdQueryHandler>();

        return services;
    }

    private static IServiceCollection AddCommandHandlers(this IServiceCollection services)
    {
        //services.AddScoped<ICommandHandler<SubmitOrderCommand, OrderId>, SubmitOrderCommandHandler>();

        return services;
    }

    private static IServiceCollection AddDomainEventHandlers(this IServiceCollection services)
    {
        //services.AddScoped<IDomainEventHandler<OrderSubmittedDomainEvent>, OrderSubmittedDomainEventHandler>();

        return services;
    }

    private static IServiceCollection AddIntegrataionEventHandlers(this IServiceCollection services)
    {
        //services.AddScoped<IIntegrationEventHandler<ProductPublishedIntegrationEvent>, 
        //    ProductPublishedIntegrationEventHandler>();

        //services.AddScoped<IIntegrationEventHandler<UserUpsertedIntegrationEvent>,
        //    UserUpsertedIntegrationEventHandler>();

        return services;
    }

    private static IServiceCollection AddValidators(this IServiceCollection services)
    {
        //services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        return services;
    }
}
