using Courses.Api.Infrastructure.ModelBinders;

namespace Courses.Api.Infrastructure;

public static class ModelBindingExtensions
{
    /// <summary>
    /// Registers the SingleValueObjectModelBinderProvider to automatically bind ISingleValueObject types from route and query parameters.
    /// </summary>
    public static IServiceCollection AddValueObjectModelBinding(this IServiceCollection services)
    {
        services.AddControllers(options =>
        {
            // Insert at the beginning to prioritize custom binding
            options.ModelBinderProviders.Insert(0, new SingleValueObjectModelBinderProvider());
        });

        return services;
    }
}
