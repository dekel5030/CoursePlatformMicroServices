using System.Text;
using Common.Web.Errors;
using CourseService.Data;
using CourseService.Data.CoursesRepo;
using CourseService.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

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

        services.AddAuthentication("Bearer")
            .AddJwtBearer("Bearer", options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = configuration["Jwt:Issuer"],

                    ValidateAudience = true,
                    ValidAudience = configuration["Jwt:Audience"],

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!)),

                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero 
                };
            });

        services.AddAuthorization();
        return services;
    }
}