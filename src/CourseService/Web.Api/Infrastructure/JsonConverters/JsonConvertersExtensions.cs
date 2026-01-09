namespace Courses.Api.Infrastructure.JsonConverters;

public static class JsonConvertersExtensions
{
    public static IServiceCollection AddValueObjectConverter(this IServiceCollection services)
    {
        var factory = new SingleValueObjectJsonConverterFactory();

        services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.Converters.Add(factory);
        });

        return services;
    }

}