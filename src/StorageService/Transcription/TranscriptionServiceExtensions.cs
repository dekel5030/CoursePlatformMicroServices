namespace StorageService.Transcription;

internal static class TranscriptionServiceExtensions
{
    public static IServiceCollection AddOpenAiTranscriptionService(this IServiceCollection services, string apiKey)
    {
        services.AddSingleton<ITranscriptionService>(provider =>
        {
            ILogger<OpenAiTranscriptionService> logger = provider
                .GetRequiredService<ILogger<OpenAiTranscriptionService>>();

            return new OpenAiTranscriptionService(apiKey, logger);
        });
        return services;
    }
}
