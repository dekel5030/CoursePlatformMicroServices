using Common.Web.Errors;
using CourseService.Data;
using CourseService.Data.CoursesRepo;
using CourseService.Services;
using Microsoft.EntityFrameworkCore;

namespace CourseService.Extentions;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddCourseServiceDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<CourseDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("CourseDb")));

        
        services.AddScoped<ICourseRepository, CourseRepository>();
        services.AddScoped<ICourseService, CourseService.Services.CourseService>();

        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

        services.AddLocalization();
        services.AddSingleton<ProblemDetailsFactory>();

        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        return services;
    }
}