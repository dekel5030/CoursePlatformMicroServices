using Courses.Domain.Courses.Primitives;
using Courses.Domain.Lessons.Primitives;
using Microsoft.OpenApi.Models;

namespace Courses.Api.Infrastructure.Routing;

internal static class RouteConstraintsExtensions
{
    internal static IServiceCollection AddRoutingConstraints(this IServiceCollection services)
    {
        services.Configure<RouteOptions>(options =>
        {
            options.ConstraintMap.Add("CourseId", typeof(ValueObjectIdConstraint<CourseId>));
            options.ConstraintMap.Add("LessonId", typeof(ValueObjectIdConstraint<LessonId>));
        });

        services.AddSwaggerGen(options =>
        {
            var uuidSchema = new OpenApiSchema { 
                Type = "string", 
                Format = "uuid" ,
            };

            options.MapType<CourseId>(() => uuidSchema);
            options.MapType<LessonId>(() => uuidSchema);
        });

        return services;
    }
}