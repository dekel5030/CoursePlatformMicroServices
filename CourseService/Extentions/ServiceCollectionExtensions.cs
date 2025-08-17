using Common;
using Common.Auth.Extentions;
using Common.Messaging.Extensions;
using Common.Messaging.Options;
using Common.Web.Errors;
using CourseService.Data;
using CourseService.Data.CoursesRepo;
using CourseService.Messaging.Consumer;
using CourseService.Messaging.Publisher;
using CourseService.Services;
using CourseService.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace CourseService.Extentions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCourseServiceDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        AddCourseDbContext(services, configuration);

        services.AddScoped<ICourseRepository, CourseRepository>();
        services.AddScoped<ICourseService, Services.CourseService>();

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
            cfg.AddConsumer<EnrollmentCancelledConsumer>();
        });

        services.AddScoped<ICourseEventPublisher, CourseEventPublisher>();

        return services;
    }
}