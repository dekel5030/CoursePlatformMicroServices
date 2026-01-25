using System.ClientModel;
using OpenAI.Audio;

namespace StorageService.Transcription;

internal class OpenAiTranscriptionService : ITranscriptionService
{
    private readonly AudioClient _audioClient;
    private readonly ILogger<OpenAiTranscriptionService> _logger;

    public OpenAiTranscriptionService(string apiKey, ILogger<OpenAiTranscriptionService> logger)
    {
        _audioClient = new AudioClient("whisper-1", apiKey);
        _logger = logger;
    }

    public async Task<string?> TranscribeAsync(string audioFilePath, CancellationToken cancellationToken = default)
    {
        if (!File.Exists(audioFilePath))
        {
            _logger.LogWarning("Audio file not found at {Path}", audioFilePath);
            return null;
        }

        try
        {
            var options = new AudioTranscriptionOptions
            {
                ResponseFormat = AudioTranscriptionFormat.Vtt,
            };

            _logger.LogInformation("Sending audio to OpenAI Whisper...");
            
            using FileStream fileStream = File.OpenRead(audioFilePath);
            string fileName = Path.GetFileName(audioFilePath);

            ClientResult<AudioTranscription> result = await _audioClient.TranscribeAudioAsync(
                fileStream,
                fileName,
                options,
                cancellationToken);

            return result.Value.Text;
        }
        catch (OperationCanceledException ex)
        {
            _logger.LogInformation(ex, "Transcription operation was cancelled.");
            return null; 
        }
        catch (IOException ex)
        {
            _logger.LogError(ex, "File access error. The file might be in use by another process (FFmpeg?). Path: {Path}", audioFilePath);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during transcription.");
            return null;
        }
    }
}
