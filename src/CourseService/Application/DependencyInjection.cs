using Courses.Application.Courses.Commands.CreateCourse;
using Courses.Application.Courses.Queries.Dtos;
using Courses.Application.Courses.Queries.GetById;
using Courses.Application.Courses.Queries.GetCourses;
using Courses.Application.Courses.Queries.GetFeatured;
using Courses.Application.Lessons.Commands.CreateLesson;
using Courses.Application.Lessons.Queries.GetById;
using Courses.Domain.Courses.Primitives;
using Courses.Domain.Lessons.Primitives;
using Kernel.Messaging.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Courses.Application;

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
        //services.AddScoped<IQueryHandler<GetCourseByIdQuery, CourseReadDto>, GetCourseByIdQueryHandler>();
        //services.AddScoped<IQueryHandler<GetCoursesQuery, PagedResponseDto<CourseReadDto>>, GetCoursesQueryHandler>();
        //services.AddScoped<IQueryHandler<GetFeaturedQuery, PagedResponseDto<CourseReadDto>>, GetFeaturedQueryHandler>();

        services.AddScoped<IQueryHandler<GetLessonByIdQuery, LessonReadDto>, GetLessonByIdQueryHandler>();

        return services;
    }

    private static IServiceCollection AddCommandHandlers(this IServiceCollection services)
    {
        //services.AddScoped<ICommandHandler<CreateCourseCommand, CourseId>, CreateCourseCommandHandler>();
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
