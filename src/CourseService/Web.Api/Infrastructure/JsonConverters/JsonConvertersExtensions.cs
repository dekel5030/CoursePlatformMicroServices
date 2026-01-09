namespace Courses.Api.Infrastructure.JsonConverters;

public static class JsonConvertersExtensions
{
    public static IServiceCollection AddJsonConverters(this IServiceCollection services)
    {
        var factory = new SingleValueObjectJsonConverterFactory();

        services.ConfigureHttpJsonOptions(options => options.SerializerOptions.Converters.Add(factory));

        services.Configure<Microsoft.AspNetCore.Mvc.JsonOptions>(options =>
            options.JsonSerializerOptions.Converters.Add(factory));

        return services;
    }
}