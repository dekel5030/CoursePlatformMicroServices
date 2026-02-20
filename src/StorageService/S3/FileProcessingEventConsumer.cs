using System.Diagnostics;
using System.Globalization;
using CoursePlatform.Contracts.StorageEvent;
using Kernel.EventBus;
using MassTransit;
using StorageService.Abstractions;
using StorageService.InternalContracts;
using StorageService.Transcription;

namespace StorageService.S3;

internal sealed class VideoProcessingRequestConsumer : IConsumer<VideoProcessingRequestEvent>
{
    private readonly IEventBus _bus;
    private readonly IStorageProvider _storage;
    private readonly ITranscriptionService _transcriptionService;
    private readonly ILogger<VideoProcessingRequestConsumer> _logger;

    public VideoProcessingRequestConsumer(
        IEventBus bus,
        IStorageProvider storage,
        ILogger<VideoProcessingRequestConsumer> logger,
        ITranscriptionService transcriptionService)
    {
        _bus = bus;
        _storage = storage;
        _logger = logger;
        _transcriptionService = transcriptionService;
    }

    public async Task Consume(ConsumeContext<VideoProcessingRequestEvent> context)
    {
        VideoProcessingRequestEvent request = context.Message;
        _logger.LogInformation("Starting video processing request for: {Key} (Ref: {RefId})", request.FileKey, request.ReferenceId);

        string uniqueId = Guid.NewGuid().ToString();
        string tempInputPath = Path.Combine(Path.GetTempPath(), $"{uniqueId}_{Path.GetFileName(request.FileKey)}");
        string tempOutputDir = Path.Combine(Path.GetTempPath(), uniqueId);

        Directory.CreateDirectory(tempOutputDir);

        try
        {
            await DownloadFileAsync(request.Bucket, request.FileKey, tempInputPath, context.CancellationToken);

            double durationSeconds = await GetVideoDurationAsync(tempInputPath);
            if (durationSeconds <= 0)
            {
                throw new VideoProcessingException($"Invalid video duration for: {request.FileKey}");
            }

            await RunFfmpegPipelineAsync(tempInputPath, tempOutputDir);

            string? transcriptKey = await CreateAndUploadTranscriptionAsync(
                request.Bucket,
                request.FileKey,
                tempOutputDir,
                context.CancellationToken);

            string processedBaseKey = $"processed/{Path.GetFileNameWithoutExtension(request.FileKey)}";
            List<string> uploadedFiles = await UploadProcessedArtifactsAsync(
                request.Bucket,
                tempOutputDir,
                processedBaseKey,
                context.CancellationToken);

            string masterKey = uploadedFiles
                .FirstOrDefault(f => 
                    f.EndsWith("master.m3u8", StringComparison.OrdinalIgnoreCase)) ?? uploadedFiles[0];

            await _bus.PublishAsync(new VideoProcessingCompletedEvent(
                ReferenceId: request.ReferenceId,
                ReferenceType: request.ReferenceType,
                OwnerService: request.OwnerService,
                MasterFileKey: masterKey,
                DurationSeconds: durationSeconds,
                TranscriptKey: transcriptKey,
                Bucket: request.Bucket
            ), context.CancellationToken);

            _logger.LogInformation("Video processing completed successfully for: {Key}", request.FileKey);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Video processing failed for: {Key}", request.FileKey);
            throw;
        }
        finally
        {
            CleanupTemporaryFiles(tempInputPath, tempOutputDir);
        }
    }

    #region Private Processing Steps (Clean Code)

    private async Task DownloadFileAsync(string bucket, string key, string dest, CancellationToken ct)
    {
        ObjectResponse response = await _storage.GetObjectAsync(bucket, key, ct);
        if (response?.Content == null)
        {
            throw new FileNotFoundException(key);
        }

        using FileStream fs = File.OpenWrite(dest);
        await response.Content.CopyToAsync(fs, ct);
    }

    private async Task RunFfmpegPipelineAsync(string input, string outputDir)
    {
        _logger.LogDebug("Executing FFmpeg pipeline...");
        string args = $"-i \"{input}\" -filter_complex \"[0:v]split=3[v1][v2][v3];[v1]scale=w=1920:h=1080:force_original_aspect_ratio=decrease,scale=w=trunc(iw/2)*2:h=trunc(ih/2)*2[v1out];[v2]scale=w=1280:h=720:force_original_aspect_ratio=decrease,scale=w=trunc(iw/2)*2:h=trunc(ih/2)*2[v2out];[v3]scale=w=640:h=360:force_original_aspect_ratio=decrease,scale=w=trunc(iw/2)*2:h=trunc(ih/2)*2[v3out]\" -map \"[v1out]\" -c:v:0 libx264 -pix_fmt yuv420p -b:v:0 5000k -maxrate:v:0 5350k -bufsize:v:0 7500k -map \"[v2out]\" -c:v:1 libx264 -pix_fmt yuv420p -b:v:1 2800k -maxrate:v:1 2996k -bufsize:v:1 4200k -map \"[v3out]\" -c:v:2 libx264 -pix_fmt yuv420p -b:v:2 800k -maxrate:v:2 856k -bufsize:v:2 1200k -map a:0 -c:a:0 aac -b:a:0 192k -map a:0 -c:a:1 aac -b:a:1 128k -map a:0 -c:a:2 aac -b:a:2 96k -f hls -hls_time 6 -hls_playlist_type vod -hls_flags independent_segments -hls_segment_type mpegts -var_stream_map \"v:0,a:0 v:1,a:1 v:2,a:2\" -master_pl_name master.m3u8 -hls_segment_filename \"{outputDir}/stream_%v_data%03d.ts\" \"{outputDir}/stream_%v.m3u8\" -map a:0 -vn -acodec libmp3lame -q:a 4 \"{outputDir}/audio.mp3\"";

        await ExecuteProcessAsync("ffmpeg", args);
    }

    private async Task<string?> CreateAndUploadTranscriptionAsync(
        string bucket, 
        string originalKey, 
        string outputDir, 
        CancellationToken ct)
    {
        string audioPath = Path.Combine(outputDir, "audio.mp3");
        if (!File.Exists(audioPath))
        {
            return null;
        }

        string? vttContent = await _transcriptionService.TranscribeAsync(audioPath, ct);
        if (string.IsNullOrEmpty(vttContent))
        {
            return null;
        }

        string vttFileName = $"{Path.GetFileNameWithoutExtension(originalKey)}.vtt";
        string vttKey = $"processed/{Path.GetFileNameWithoutExtension(originalKey)}/{vttFileName}";

        using var ms = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(vttContent));
        await _storage.UploadObjectAsync(ms, vttKey, "text/vtt", ms.Length, bucket, ct);

        return vttKey;
    }

    private async Task<List<string>> UploadProcessedArtifactsAsync(string bucket, string outputDir, string baseKey, CancellationToken ct)
    {
        var files = Directory.GetFiles(outputDir)
            .Where(f => !f.EndsWith(".mp3", StringComparison.OrdinalIgnoreCase))
            .ToList();

        var uploadedKeys = new List<string>();

        foreach (string? file in files)
        {
            string fileName = Path.GetFileName(file);
            string key = $"{baseKey}/{fileName}";
            using FileStream stream = File.OpenRead(file);
            await _storage.UploadObjectAsync(stream, key, GetContentType(fileName), stream.Length, bucket, ct);
            uploadedKeys.Add(key);
        }
        return uploadedKeys;
    }

    private static async Task ExecuteProcessAsync(string fileName, string args)
    {
        var psi = new ProcessStartInfo
        {
            FileName = fileName,
            Arguments = args,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using Process process = Process.Start(psi)
            ?? throw new InvalidOperationException($"Could not start external process: {fileName}");

        await process.WaitForExitAsync();

        if (process.ExitCode != 0)
        {
            string errorOutput = await process.StandardError.ReadToEndAsync();
            throw new VideoProcessingException($"{fileName} failed with exit code {process.ExitCode}. Details: {errorOutput}");
        }
    }

    private static async Task<double> GetVideoDurationAsync(string path)
    {
        var psi = new ProcessStartInfo
        {
            FileName = "ffprobe",
            Arguments = $"-v error -show_entries format=duration -of default=noprint_wrappers=1:nokey=1 \"{path}\"",
            RedirectStandardOutput = true,
            UseShellExecute = false
        };
        using var process = Process.Start(psi);
        string output = await process!.StandardOutput.ReadToEndAsync();
        return double.TryParse(output.Trim(), CultureInfo.InvariantCulture, out double res) ? res : 0;
    }

    private static string GetContentType(string fileName) => Path.GetExtension(fileName).ToLower() switch
    {
        ".m3u8" => "application/x-mpegURL",
        ".ts" => "video/MP2T",
        ".vtt" => "text/vtt",
        _ => "application/octet-stream"
    };

    private static void CleanupTemporaryFiles(string input, string dir)
    {
        if (File.Exists(input))
        {
            File.Delete(input);
        }

        if (Directory.Exists(dir))
        {
            Directory.Delete(dir, true);
        }
    }
    #endregion
}
