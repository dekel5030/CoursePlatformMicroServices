using Common;
using Common.Messaging.Extensions;
using Common.Messaging.Options;
using Common.Web.Errors;
using EnrollmentService.Data;
using EnrollmentService.Messaging.Publishers;
using EnrollmentService.Options;
using EnrollmentService.Profiles;
using EnrollmentService.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace EnrollmentService.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEnrollmentDependencies(this IServiceCollection services)
    {
        services.SetupOptions();
        services.AddEnrollmentDbContext();
        services.AddScoped<IEnrollmentService, Services.EnrollmentService>();
        services.AddScoped<IEnrollmentRepository, EnrollmentRepository>();
        services.AddLocalization();
        services.AddSingleton<ProblemDetailsFactory>();
        services.AddAutoMapper(cfg => { }, typeof(EnrollmentProfile).Assembly);

        services.AddAppMessaging();
        services.AddScoped<IEnrollmentEventPublisher, EnrollmentEventPublisher>();

        services.AddAppSwagger();

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

        services.AddMassTransitRabbitMq();
        
        return services;
    }
}
