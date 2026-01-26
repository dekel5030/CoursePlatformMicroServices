namespace StorageService.Transcription;

internal interface ITranscriptionService
{
    Task<string?> TranscribeAsync(string audioFilePath, CancellationToken cancellationToken = default);
}
