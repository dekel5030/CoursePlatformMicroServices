using Amazon.S3;
using Microsoft.Extensions.Options;
using StorageService.Abstractions;

namespace StorageService.S3;

internal static class S3Extensions
{
    public static IServiceCollection ConfigureS3(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        services.Configure<S3Options>(configuration.GetSection(S3Options.SectionName));

        services.AddSingleton<IAmazonS3>(sp =>
        {
            S3Options options = sp.GetRequiredService<IOptions<S3Options>>().Value;

            var config = new AmazonS3Config
            {
                ServiceURL = options.ServiceUrl,
                ForcePathStyle = true,
                AuthenticationRegion = options.Region,
            };

            return new AmazonS3Client(options.AccessKey, options.SecretKey, config);
        });

        services.AddScoped<IStorageProvider, S3StorageProvider>();

        return services;
    }
}
