using Common;
using Common.Auth.Extentions;
using Common.Web.Errors;
using CourseService.Data;
using CourseService.Data.CoursesRepo;
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
        services.AddScoped<ICourseService, CourseService.Services.CourseService>();

        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

        services.AddLocalization();
        services.AddSingleton<ProblemDetailsFactory>();

        services.AddHttpContextAccessor();

        services.AddJwtAuthentication(configuration);
        services.AddPermissionAuthorization();

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
}