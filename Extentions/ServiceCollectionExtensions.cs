using EnrollmentService.Data;
using EnrollmentService.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace EnrollmentService.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEnrollmentDependencies(this IServiceCollection services)
    {
        services.AddEnrollmentDbContext();

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
