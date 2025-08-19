using Common;
using Common.Messaging.Extensions;
using Common.Messaging.Options;
using Common.Web.Errors;
using Courses.Contracts.Events;
using EnrollmentService.Data;
using EnrollmentService.Data.Repositories.Implementations;
using EnrollmentService.Data.Repositories.Interfaces;
using EnrollmentService.Messaging.Consumers;
using EnrollmentService.Messaging.Publishers;
using EnrollmentService.Options;
using EnrollmentService.Profiles;
using EnrollmentService.Services;
using EnrollmentService.Services.EnrollmentMessageHandler;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Users.Contracts.Events;

namespace EnrollmentService.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEnrollmentDependencies(this IServiceCollection services)
    {
        services.SetupOptions();
        services.AddEnrollmentDbContext();
        services.AddRepositories();
        services.AddAppBusinessLogics();
        services.AddLocalization();
        services.AddSingleton<ProblemDetailsFactory>();
        services.AddAutoMapper(cfg => { }, typeof(EnrollmentProfile).Assembly);
        services.AddScoped<IEnrollmentEventPublisher, EnrollmentEventPublisher>();

        services.AddAppMessaging();

        services.AddAppSwagger();

        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ICourseRepository, CourseRepository>();
        services.AddScoped<IEnrollmentRepository, EnrollmentRepository>();

        return services;
    }

    private static IServiceCollection AddAppBusinessLogics(this IServiceCollection services)
    {
        services.AddScoped<IEnrollmentService, Services.EnrollmentService>();
        services.AddScoped<IEnvelopeHandler<CourseUpsertedV1>, CourseUpsertedHandler>();
        services.AddScoped<IEnvelopeHandler<UserUpsertedV1>, UserUpsertedHandler>();    

        return services;
    }

    private static IServiceCollection AddEnrollmentDbContext(this IServiceCollection services)
    {
        services.AddOptions<EnrollmentDbOptions>()
            .BindConfiguration(EnrollmentDbOptions.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddDbContext<EnrollmentDbContext>((sp, options) =>
        {
            var dbOptions = sp.GetRequiredService<IOptions<EnrollmentDbOptions>>().Value;

            options.UseNpgsql(dbOptions.BuildConnectionString());
        });

        return services;
    }

    private static IServiceCollection SetupOptions(this IServiceCollection services)
    {
        services.AddOptions<PaginationOptions>()
            .BindConfiguration(PaginationOptions.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        return services;
    }

    private static IServiceCollection AddAppMessaging(this IServiceCollection services)
    {
        services.AddOptions<RabbitMqOptions>()
            .BindConfiguration(RabbitMqOptions.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddOptions<MassTransitOptions>()
            .BindConfiguration(MassTransitOptions.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddMassTransitRabbitMq(reg =>
        {
            reg.AddConsumer<CourseUpsertedConsumer>((context, config) =>
            {
                config.UseInMemoryOutbox(context);
            });

            reg.AddConsumer<UserUpsertedConsumer>((context, config) =>
            {
                config.UseInMemoryOutbox(context);
            });
        });
        
        return services;
    }
}
