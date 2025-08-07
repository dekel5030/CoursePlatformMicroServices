using Common.Web.Errors;
using CourseService.Data;
using CourseService.Data.CoursesRepo;
using CourseService.Security;
using CourseService.Services;
using CourseService.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace CourseService.Extentions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCourseServiceDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        // === Database Context ===
        services.AddOptions<CourseDbSettings>()
                .Bind(configuration.GetSection(CourseDbSettings.SectionName));
        services.AddDbContext<CourseDbContext>((sp, options) =>
        {
            var dbSettings = sp.GetRequiredService<IOptions<CourseDbSettings>>().Value;
            options.UseNpgsql(dbSettings.BuildConnectionString());
        });

        
        services.AddScoped<ICourseRepository, CourseRepository>();
        services.AddScoped<ICourseService, CourseService.Services.CourseService>();

        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

        services.AddLocalization();
        services.AddSingleton<ProblemDetailsFactory>();

        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();


        // === Security ===
        services.ConfigureOptions<JwtOptionsSetup>();
        services.ConfigureOptions<JwtBearerOptionsSetup>();
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer();

        services.AddAuthorization();
        return services;
    }
}