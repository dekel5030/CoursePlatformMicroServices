using EnrollmentService.Data;
using EnrollmentService.Options;
using EnrollmentService.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace EnrollmentService.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEnrollmentDependencies(this IServiceCollection services)
    {
        services.AddEnrollmentDbContext();
        services.AddScoped<IEnrollmentService, Services.EnrollmentService>();
        //services.AddAutoMapper(typeof(Program));
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
}
