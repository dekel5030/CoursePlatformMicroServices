using Common;
using Common.Auth.Extentions;
using Common.Messaging.Extensions;
using Common.Messaging.Options;
using Common.Web.Errors;
using CourseService.Data;
using CourseService.Data.CoursesRepo;
using CourseService.Data.EnrollmentsRepo;
using CourseService.Data.UnitOfWork;
using CourseService.Messaging.Consumer;
using CourseService.Messaging.Publisher;
using CourseService.Services;
using CourseService.Services.Handlers;
using CourseService.Settings;
using Enrollments.Contracts.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace CourseService.Extentions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCourseServiceDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddCourseDbContext(configuration);
        services.AddRepositories();
        services.AddBusinessServices();

        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

        services.AddLocalization();
        services.AddSingleton<ProblemDetailsFactory>();

        services.AddHttpContextAccessor();

        services.AddJwtAuthentication(configuration);
        services.AddPermissionAuthorization();

        services.AddMessageQueue();

        services.AddAppSwagger();

        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<ICourseRepository, CourseRepository>();
        services.AddScoped<IEnrollmentsRepo, EnrollmentsRepo>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }

    private static IServiceCollection AddBusinessServices(this IServiceCollection services)
    {
        services.AddScoped<ICourseService, Services.CourseService>();
        services.AddScoped<IEnvelopeHandler<EnrollmentUpsertedV1>, EnrollmentUpsertedHandler>();

        return services;
    }

    private static IServiceCollection AddCourseDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<CourseDbOptions>()
                .Bind(configuration.GetSection(CourseDbOptions.SectionName));
        services.AddDbContext<CourseDbContext>((sp, options) =>
        {
            var dbSettings = sp.GetRequiredService<IOptions<CourseDbOptions>>().Value;
            options.UseNpgsql(dbSettings.BuildConnectionString());
        });

        return services;
    }

    private static IServiceCollection AddMessageQueue(this IServiceCollection services)
    {
        services.AddOptions<RabbitMqOptions>()
                .BindConfiguration(RabbitMqOptions.SectionName)
                .ValidateDataAnnotations()
                .ValidateOnStart();

        services.AddOptions<MassTransitOptions>()
                .BindConfiguration(MassTransitOptions.SectionName)
                .ValidateDataAnnotations()
                .ValidateOnStart();

        services.AddMassTransitRabbitMq(cfg =>
        {
            cfg.AddConsumer<EnrollmentUpsertedConsumer>();
        });

        services.AddScoped<ICourseEventPublisher, CourseEventPublisher>();

        return services;
    }
}