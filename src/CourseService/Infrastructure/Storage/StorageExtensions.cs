using Courses.Application.Abstractions.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace Courses.Infrastructure.Storage;

internal static class StorageExtensions
{
    public static IServiceCollection AddStorage(this IServiceCollection services)
    {
        services.AddOptions<StorageOptions>().BindConfiguration(StorageOptions.SectionName);
        services.AddSingleton<IStorageUrlResolver, StorageUrlResolver>();

        services.AddSingleton<IObjectStorageService>(sp => (StorageUrlResolver)sp.GetRequiredService<IStorageUrlResolver>());
        services.AddScoped<ITranscriptFileService, TranscriptFileService>();
        return services;
    }
}
