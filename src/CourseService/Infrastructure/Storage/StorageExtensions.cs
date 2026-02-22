using Courses.Application.Abstractions.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace Courses.Infrastructure.Storage;

internal static class StorageExtensions
{
    public static IServiceCollection AddStorage(this IServiceCollection services)
    {
        services.AddOptions<StorageOptions>().BindConfiguration(StorageOptions.SectionName);
        services.AddSingleton<IStorageUrlResolver, StorageService>();

        services.AddSingleton<IObjectStorageService>(sp => (StorageService)sp.GetRequiredService<IStorageUrlResolver>());
        services.AddSingleton<IStorageBucketResolver, StorageBucketResolver>();
        services.AddScoped<ITranscriptFileService, TranscriptFileService>();
        return services;
    }
}
