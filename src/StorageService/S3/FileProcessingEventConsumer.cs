using System.Diagnostics;
using System.Globalization;
using CoursePlatform.Contracts.StorageEvent;
using Kernel.EventBus;
using MassTransit;
using StorageService.Abstractions;
using StorageService.InternalContracts;
using StorageService.Transcription;
using static MassTransit.Monitoring.Performance.BuiltInCounters;

namespace StorageService.S3;

internal sealed class FileProcessingEventConsumer
    : IEventConsumer<FileProcessingEvent>, IConsumer<FileProcessingEvent>
{
    private readonly IEventBus _bus;
    private readonly IStorageProvider _storage;
    private readonly ITranscriptionService _transcriptionService;
    private readonly ILogger<FileProcessingEventConsumer> _logger;

    public FileProcessingEventConsumer(
        IEventBus bus,
        IStorageProvider storage,
        ILogger<FileProcessingEventConsumer> logger,
        ITranscriptionService transcriptionService)
    {
        _bus = bus;
        _storage = storage;
        _logger = logger;
        _transcriptionService = transcriptionService;
    }

    public async Task HandleAsync(FileProcessingEvent message, CancellationToken cancellationToken = default)
    {
        if (IsImage(message.ContentType))
        {
            await HandleImageAsync(message, cancellationToken);
            return;
        }

        await HandleVideoProcessingAsync(message, cancellationToken);
    }

    private async Task HandleVideoProcessingAsync(
        FileProcessingEvent message,
        CancellationToken cancellationToken = default)
    {
        string uniqueId = Guid.NewGuid().ToString();
        string tempInputPath = Path.Combine(Path.GetTempPath(), $"{uniqueId}_{Path.GetFileName(message.FileKey)}");
        string tempOutputDir = Path.Combine(Path.GetTempPath(), uniqueId);

        Directory.CreateDirectory(tempOutputDir);

        try
        {
            _logger.LogInformation("Processing video: {Key}", message.FileKey);

            await DownloadFileToLocalAsync(message.Bucket, message.FileKey, tempInputPath, cancellationToken);

            (double duration, string? formatted) = await GetVideoMetadataAsync(tempInputPath);
            if (duration <= 0)
            {
                throw new VideoProcessingException($"Invalid video duration ({duration}s) for file: {message.FileKey}");
            }

            await ProcessVideoAndAudioAsync(tempInputPath, tempOutputDir);

            string? vttKey = await CreateAndUploadTranscriptionAsync(message, tempOutputDir, cancellationToken);

            List<string> uploadedFiles = await UploadProcessedFilesParallelAsync(message, tempOutputDir, cancellationToken);

            if (uploadedFiles.Count == 0)
            {
                throw new VideoProcessingException($"No files were generated during FFmpeg processing for: {message.FileKey}");
            }

            await PublishCompletionEventAsync(message, uploadedFiles, vttKey, duration, formatted, cancellationToken);
        }
        catch (Exception ex) when (ex is not VideoProcessingException)
        {
            throw new VideoProcessingException($"An unexpected error occurred while processing {message.FileKey}", ex);
        }
        finally
        {
            CleanupFiles(tempInputPath, tempOutputDir);
        }
    }

    private async Task DownloadFileToLocalAsync(
        string bucket,
        string key,
        string dest,
        CancellationToken cancellationToken = default)
    {
        ObjectResponse response = await _storage.GetObjectAsync(bucket, key, cancellationToken);
        if (response?.Content == null)
        {
            throw new FileNotFoundException($"Source file not found or empty: {key} in bucket {bucket}");
        }

        using FileStream fileStream = File.OpenWrite(dest);
        await response.Content.CopyToAsync(fileStream, cancellationToken);
    }

    private static async Task<(double seconds, string formatted)> GetVideoMetadataAsync(string path)
    {
        double seconds = await GetVideoDurationAsync(path);
        string formatted = TimeSpan.FromSeconds(seconds).ToString(@"hh\:mm\:ss");
        return (seconds, formatted);
    }

    private async Task ProcessVideoAndAudioAsync(string input, string outputDir)
    {
        _logger.LogInformation("Running FFmpeg for HLS and MP3 extraction...");
        await RunFfmpegAsync(input, outputDir);
    }

    private async Task<string?> CreateAndUploadTranscriptionAsync(
        FileProcessingEvent message, 
        string outputDir, 
        CancellationToken cancellationToken = default)
    {
        string audioPath = Path.Combine(outputDir, "audio.mp3");
        if (!File.Exists(audioPath))
        {
            return null;
        }

        _logger.LogInformation("Starting transcription...");
        string? vttContent = await _transcriptionService.TranscribeAsync(audioPath, cancellationToken);

        if (string.IsNullOrEmpty(vttContent))
        {
            return null;
        }

        string vttFileName = $"{Path.GetFileNameWithoutExtension(message.FileKey)}.vtt";
        string vttKey = $"processed/{Path.GetFileNameWithoutExtension(message.FileKey)}/{vttFileName}";
        string localVttPath = Path.Combine(outputDir, vttFileName);

        await File.WriteAllTextAsync(localVttPath, vttContent, cancellationToken);

        using FileStream vttStream = File.OpenRead(localVttPath);
        await _storage.UploadObjectAsync(vttStream, vttKey, "text/vtt", vttStream.Length, message.Bucket);

        return vttKey;
    }

    private async Task<List<string>> UploadProcessedFilesParallelAsync(
        FileProcessingEvent message,
        string outputDir,
        CancellationToken cancellationToken = default)
    {
        string processedBaseKey = $"processed/{Path.GetFileNameWithoutExtension(message.FileKey)}";
        var filesToUpload = Directory.GetFiles(outputDir)
            .Where(f => !f.EndsWith(".mp3", StringComparison.OrdinalIgnoreCase) &&
                        !f.EndsWith(".vtt", StringComparison.OrdinalIgnoreCase))
            .ToList();

        var uploadedKeys = new List<string>();
        using var semaphore = new SemaphoreSlim(10);

        IEnumerable<Task> tasks = filesToUpload.Select(async filePath =>
        {
            await semaphore.WaitAsync(cancellationToken);
            try
            {
                string fileName = Path.GetFileName(filePath);
                string key = $"{processedBaseKey}/{fileName}";
                using FileStream stream = File.OpenRead(filePath);
                await _storage.UploadObjectAsync(stream, key, GetContentType(fileName), stream.Length, message.Bucket);

                lock (uploadedKeys)
                { uploadedKeys.Add(key); }
            }
            finally { semaphore.Release(); }
        });

        await Task.WhenAll(tasks);
        return uploadedKeys;
    }

    private async Task PublishCompletionEventAsync(
        FileProcessingEvent message,
        List<string> files,
        string? vttKey,
        double durSec,
        string durForm,
        CancellationToken cancellationTokent = default)
    {
        string processedBaseKey = $"processed/{Path.GetFileNameWithoutExtension(message.FileKey)}";
        string masterKey = files.FirstOrDefault(f => f
            .EndsWith("master.m3u8", StringComparison.InvariantCultureIgnoreCase)) ?? files[0];

        await _bus.PublishAsync(new FileUploadedEvent
        {
            FileKey = masterKey,
            Bucket = message.Bucket,
            ContentType = "application/x-mpegURL",
            FileSize = message.FileSize,
            OwnerService = message.OwnerService,
            ReferenceId = message.ReferenceId,
            ReferenceType = message.ReferenceType,
            Metadata = new Dictionary<string, string>
            {
                { "RawVideoKey", message.FileKey },
                { "AudioKey", $"{processedBaseKey}/audio.mp3" },
                { "DurationSeconds", durSec.ToString(CultureInfo.InvariantCulture) },
                { "Duration", durForm },
                { "TranscriptKey", vttKey ?? "" }
            }
        }, cancellationTokent);
    }

    private void CleanupFiles(string input, string outputDir)
    {
        try
        {
            if (File.Exists(input))
            {
                File.Delete(input);
            }

            if (Directory.Exists(outputDir))
            {
                Directory.Delete(outputDir, true);
            }
        }
        catch (Exception ex) { _logger.LogWarning(ex, "Cleanup failed."); }
    }

    private static bool IsImage(string contentType)
    {
        return contentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase);
    }

    private async Task HandleImageAsync(
        FileProcessingEvent message,
        CancellationToken cancellationToken = default)
    {
        await _bus.PublishAsync(new FileUploadedEvent
        {
            FileKey = message.FileKey,
            Bucket = message.Bucket,
            ContentType = message.ContentType,
            FileSize = message.FileSize,
            OwnerService = message.OwnerService,
            ReferenceId = message.ReferenceId,
            ReferenceType = message.ReferenceType,
            Metadata = new Dictionary<string, string>()
        }, cancellationToken);
    }

    private static async Task<double> GetVideoDurationAsync(string inputPath)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = "ffprobe",
            Arguments = $"-v error -show_entries format=duration -of default=noprint_wrappers=1:nokey=1 \"{inputPath}\"",
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = Process.Start(startInfo);
        if (process == null)
        {
            return 0;
        }

        string output = await process.StandardOutput.ReadToEndAsync();
        await process.WaitForExitAsync();

        return double.TryParse(
            output.Trim(),
            NumberStyles.Any,
            CultureInfo.InvariantCulture,
            out double duration) ? duration : 0;
    }

    private static async Task RunFfmpegAsync(string inputPath, string outputDir)
    {
        string arguments = $"-i \"{inputPath}\" -filter_complex \"[0:v]split=3[v1][v2][v3];[v1]scale=w=1920:h=1080:force_original_aspect_ratio=decrease,scale=w=trunc(iw/2)*2:h=trunc(ih/2)*2[v1out];[v2]scale=w=1280:h=720:force_original_aspect_ratio=decrease,scale=w=trunc(iw/2)*2:h=trunc(ih/2)*2[v2out];[v3]scale=w=640:h=360:force_original_aspect_ratio=decrease,scale=w=trunc(iw/2)*2:h=trunc(ih/2)*2[v3out]\" -map \"[v1out]\" -c:v:0 libx264 -pix_fmt yuv420p -b:v:0 5000k -maxrate:v:0 5350k -bufsize:v:0 7500k -map \"[v2out]\" -c:v:1 libx264 -pix_fmt yuv420p -b:v:1 2800k -maxrate:v:1 2996k -bufsize:v:1 4200k -map \"[v3out]\" -c:v:2 libx264 -pix_fmt yuv420p -b:v:2 800k -maxrate:v:2 856k -bufsize:v:2 1200k -map a:0 -c:a:0 aac -b:a:0 192k -map a:0 -c:a:1 aac -b:a:1 128k -map a:0 -c:a:2 aac -b:a:2 96k -f hls -hls_time 6 -hls_playlist_type vod -hls_flags independent_segments -hls_segment_type mpegts -var_stream_map \"v:0,a:0 v:1,a:1 v:2,a:2\" -master_pl_name master.m3u8 -hls_segment_filename \"{outputDir}/stream_%v_data%03d.ts\" \"{outputDir}/stream_%v.m3u8\" -map a:0 -vn -acodec libmp3lame -q:a 4 \"{outputDir}/audio.mp3\"";

        var startInfo = new ProcessStartInfo
        {
            FileName = "ffmpeg",
            Arguments = arguments,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using Process process = Process.Start(startInfo)
            ?? throw new VideoProcessingException("Failed to start FFmpeg process.");

        string error = await process.StandardError.ReadToEndAsync();
        await process.WaitForExitAsync();

        if (process.ExitCode != 0)
        {
            throw new InvalidDataException($"FFmpeg failed with exit code {process.ExitCode}. Error details: {error}");
        }
    }

    private static string GetContentType(string fileName) => Path.GetExtension(fileName).ToLower() switch
    {
        ".m3u8" => "application/x-mpegURL",
        ".ts" => "video/MP2T",
        ".mp3" => "audio/mpeg",
        ".vtt" => "text/vtt",
        _ => "application/octet-stream"
    };

    public Task Consume(ConsumeContext<FileProcessingEvent> context) => HandleAsync(context.Message, context.CancellationToken);
}

#pragma warning disable CA1515 // Consider making public types internal
public class VideoProcessingException : Exception
#pragma warning restore CA1515 // Consider making public types internal
{
    public VideoProcessingException(string message) : base(message) { }
    public VideoProcessingException(string message, Exception inner) : base(message, inner) { }

    public VideoProcessingException()
    {
    }
}
