using CourseService.Data;
using CourseService.Data.CoursesRepo;
using Microsoft.EntityFrameworkCore;

namespace CourseService.Extentions;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddCourseServiceDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<CourseDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("CourseDb")));

        services.AddScoped<ICourseRepository, CourseRepository>();

        return services;
    }
}