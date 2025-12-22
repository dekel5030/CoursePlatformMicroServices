using Application.Courses.Commands.CreateCourse;
using Application.Courses.Queries.Dtos;
using Application.Courses.Queries.GetById;
using Application.Courses.Queries.GetCourses;
using Application.Courses.Queries.GetFeatured;
using Application.Lessons.Commands.CreateLesson;
using Application.Lessons.Queries.GetById;
using Domain.Courses.Primitives;
using Domain.Lessons.Primitives;
using Kernel.Messaging.Abstractions;
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
        services.AddScoped<IQueryHandler<GetCoursesQuery, PagedResponseDto<CourseReadDto>>, GetCoursesQueryHandler>();
        services.AddScoped<IQueryHandler<GetFeaturedQuery, PagedResponseDto<CourseReadDto>>, GetFeaturedQueryHandler>();

        services.AddScoped<IQueryHandler<GetLessonByIdQuery, LessonReadDto>, GetLessonByIdQueryHandler>();

        return services;
    }

    private static IServiceCollection AddCommandHandlers(this IServiceCollection services)
    {
        services.AddScoped<ICommandHandler<CreateCourseCommand, CourseId>, CreateCourseCommandHandler>();
        services.AddScoped<ICommandHandler<CreateLessonCommand, LessonId>, CreateLessonCommandHandler>();

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
