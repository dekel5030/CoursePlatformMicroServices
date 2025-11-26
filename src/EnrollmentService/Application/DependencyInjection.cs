using Application.Abstractions.Messaging;
using Application.Courses.IntegrationEvents;
using Application.Enrollments.Commands.CreateEnrollment;
using Application.Enrollments.Commands.DeleteEnrollment;
using Application.Enrollments.DomainEvents;
using Application.Enrollments.Queries.Dtos;
using Application.Enrollments.Queries.GetEnrollmentById;
using Application.Enrollments.Queries.GetEnrollments;
using Application.Users.IntegrationEvents;
using Domain.Enrollments.Events;
using Domain.Enrollments.Primitives;
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

        return services;
    }

    private static IServiceCollection AddQueryHandlers(this IServiceCollection services)
    {
        services.AddScoped<IQueryHandler<GetEnrollmentByIdQuery, EnrollmentReadDto>,
            GetEnrollmentByIdQueryHandler>();
        services.AddScoped<IQueryHandler<GetEnrollmentsQuery, PagedResponse<EnrollmentReadDto>>,
            GetEnrollmentsQueryHandler>();

        return services;
    }

    private static IServiceCollection AddCommandHandlers(this IServiceCollection services)
    {
        services.AddScoped<ICommandHandler<CreateEnrollmentCommand, EnrollmentId>,
            CreateEnrollmentCommandHandler>();
        services.AddScoped<ICommandHandler<DeleteEnrollmentCommand>,
            DeleteEnrollmentCommandHandler>();

        return services;
    }

    private static IServiceCollection AddDomainEventHandlers(this IServiceCollection services)
    {
        services.AddScoped<IDomainEventHandler<EnrollmentCreatedDomainEvent>,
            EnrollmentCreatedDomainEventHandler>();

        return services;
    }

    private static IServiceCollection AddIntegrationEventHandlers(this IServiceCollection services)
    {
        services.AddScoped<IIntegrationEventHandler<UserUpsertedIntegrationEvent>,
            UserUpsertedIntegrationEventHandler>();
        services.AddScoped<IIntegrationEventHandler<CourseUpsertedIntegrationEvent>,
            CourseUpsertedIntegrationEventHandler>();

        return services;
    }
}
