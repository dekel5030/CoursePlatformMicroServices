using System.Diagnostics;
using System.Globalization;
using CoursePlatform.Contracts.StorageEvent;
using Kernel.EventBus;
using MassTransit;
using StorageService.Abstractions;
using StorageService.InternalContracts;

namespace StorageService.S3;

internal sealed class FileProcessingEventConsumer : IEventConsumer<FileProcessingEvent>, IConsumer<FileProcessingEvent>
{
    private readonly IEventBus _bus;
    private readonly IStorageProvider _storage;
    private readonly ILogger<FileProcessingEventConsumer> _logger;

    public FileProcessingEventConsumer(IEventBus bus, IStorageProvider storage, ILogger<FileProcessingEventConsumer> logger)
    {
        _bus = bus;
        _storage = storage;
        _logger = logger;
    }

    public async Task HandleAsync(FileProcessingEvent message, CancellationToken cancellationToken = default)
    {
        if (message.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogInformation("File is an image, skipping video processing: {Key}", message.FileKey);

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

            return;
        }

        string tempInputPath = Path.GetRandomFileName();
        string tempOutputDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempOutputDir);

        try
        {
            _logger.LogInformation("Processing video: {Key}", message.FileKey);

            ObjectResponse objectResponse = await _storage
                .GetObjectAsync(message.Bucket, message.FileKey, cancellationToken);

            using (FileStream fileStream = File.OpenWrite(tempInputPath))
            {
                await objectResponse.Content.CopyToAsync(fileStream, cancellationToken);
            }

            double durationInSeconds = await GetVideoDurationAsync(tempInputPath);
            string formattedDuration = TimeSpan.FromSeconds(durationInSeconds).ToString(@"hh\:mm\:ss");

            await RunFfmpegAsync(tempInputPath, tempOutputDir);

            string processedBaseKey = $"processed/{Path.GetFileNameWithoutExtension(message.FileKey)}";

            var uploadedFiles = new List<string>();
            foreach (string filePath in Directory.GetFiles(tempOutputDir))
            {
                string fileName = Path.GetFileName(filePath);
                string key = $"{processedBaseKey}/{fileName}";
                string contentType = GetContentType(fileName);

                using FileStream stream = File.OpenRead(filePath);
                await _storage.UploadObjectAsync(stream, key, contentType, stream.Length, message.Bucket);
                uploadedFiles.Add(key);
            }

            string masterPlaylistKey = uploadedFiles
                .FirstOrDefault(f => f.EndsWith("master.m3u8", StringComparison.OrdinalIgnoreCase))
                                    ?? uploadedFiles[0];

            await _bus.PublishAsync(new FileUploadedEvent
            {
                FileKey = masterPlaylistKey,
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
                    { "DurationSeconds", durationInSeconds.ToString(CultureInfo.InvariantCulture) },
                    { "Duration", formattedDuration }
                }
            }, cancellationToken);

            _logger.LogInformation("Finished processing video: {Key}", message.FileKey);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process video: {Key}", message.FileKey);
            throw;
        }
        finally
        {
            if (File.Exists(tempInputPath))
            {
                File.Delete(tempInputPath);
            }

            if (Directory.Exists(tempOutputDir))
            {
                Directory.Delete(tempOutputDir, true);
            }
        }
    }

    private static async Task<double> GetVideoDurationAsync(string inputPath)
    {
        var processStartInfo = new ProcessStartInfo
        {
            FileName = "ffprobe",
            Arguments = $"-v error -show_entries format=duration -of default=noprint_wrappers=1:nokey=1 \"{inputPath}\"",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = new Process { StartInfo = processStartInfo };
        process.Start();

        string output = await process.StandardOutput.ReadToEndAsync();
        await process.WaitForExitAsync();

        if (double.TryParse(output.Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out double duration))
        {
            return duration;
        }

        return 0;
    }

    private static async Task RunFfmpegAsync(string inputPath, string outputDir)
    {
        string arguments =
                $"-i \"{inputPath}\" " +

                // 1. פיצול הוידאו ל-3 גדלים (1080, 720, 360)
                $"-filter_complex \"[0:v]split=3[v1][v2][v3];" +
                $"[v1]scale=w=1920:h=1080:force_original_aspect_ratio=decrease[v1out];" +
                $"[v2]scale=w=1280:h=720:force_original_aspect_ratio=decrease[v2out];" +
                $"[v3]scale=w=640:h=360:force_original_aspect_ratio=decrease[v3out]\" " +

                // 2. הגדרת הזרם הראשון (1080p)
                $"-map \"[v1out]\" -c:v:0 libx264 -b:v:0 5000k -maxrate:v:0 5350k -bufsize:v:0 7500k " +

                // 3. הגדרת הזרם השני (720p)
                $"-map \"[v2out]\" -c:v:1 libx264 -b:v:1 2800k -maxrate:v:1 2996k -bufsize:v:1 4200k " +

                // 4. הגדרת הזרם השלישי (360p)
                $"-map \"[v3out]\" -c:v:2 libx264 -b:v:2 800k -maxrate:v:2 856k -bufsize:v:2 1200k " +

                // 5. יצירת 3 ערוצי אודיו (אחד לכל וידאו)
                $"-map a:0 -c:a:0 aac -b:a:0 192k " +
                $"-map a:0 -c:a:1 aac -b:a:1 128k " +
                $"-map a:0 -c:a:2 aac -b:a:2 96k " +

                // 6. הגדרות HLS
                $"-f hls -hls_time 6 -hls_playlist_type vod -hls_flags independent_segments " +
                $"-hls_segment_type mpegts " +

                // 7. המיפוי הקריטי! מחבר בין וידאו לאודיו בכל רמה
                $"-var_stream_map \"v:0,a:0 v:1,a:1 v:2,a:2\" " +

                $"-master_pl_name master.m3u8 " +
                $"-hls_segment_filename \"{outputDir}/stream_%v_data%03d.ts\" " +
                $"\"{outputDir}/stream_%v.m3u8\" " + // פלט ה-HLS

                // 8. פלט נפרד ל-MP3 (כמו שעשית במקור, פקודה אחת שמייצרת גם וגם)
                $"-map a:0 -vn -acodec libmp3lame -q:a 4 \"{outputDir}/audio.mp3\"";

        var processStartInfo = new ProcessStartInfo
        {
            FileName = "ffmpeg",
            Arguments = arguments,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = new Process { StartInfo = processStartInfo };

        process.Start();
        string error = await process.StandardError.ReadToEndAsync();
        await process.WaitForExitAsync();

        if (process.ExitCode != 0)
        {
            throw new ArgumentException($"FFmpeg failed with exit code {process.ExitCode}: {error}");
        }
    }

    private static string GetContentType(string fileName) => Path.GetExtension(fileName) switch
    {
        ".m3u8" => "application/x-mpegURL",
        ".ts" => "video/MP2T",
        ".mp3" => "audio/mpeg",
        _ => "application/octet-stream"
    };

    public Task Consume(ConsumeContext<FileProcessingEvent> context)
    {
        return HandleAsync(context.Message, context.CancellationToken);
    }
}

