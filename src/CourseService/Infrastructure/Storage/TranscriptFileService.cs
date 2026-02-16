using System.Text;
using Courses.Application.Abstractions.Storage;
using Kernel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Courses.Infrastructure.Storage;

internal sealed class TranscriptFileService : ITranscriptFileService
{
    private readonly IStorageUrlResolver _urlResolver;
    private readonly IObjectStorageService _storageService;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<TranscriptFileService> _logger;

    public TranscriptFileService(
        IStorageUrlResolver urlResolver,
        IHttpClientFactory httpClientFactory,
        ILogger<TranscriptFileService> logger,
        IObjectStorageService storageService)
    {
        _urlResolver = urlResolver;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _storageService = storageService;
    }

    public async Task<string?> GetVttContentAsync(
        string relativePath,
        StorageCategory storageCategory,
        CancellationToken cancellationToken = default)
    {
        string url = _urlResolver.Resolve(storageCategory, relativePath).Value;
        using HttpClient client = _httpClientFactory.CreateClient();
        try
        {
            return await client.GetStringAsync(new Uri(url), cancellationToken);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Storage service is unreachable");
            return null;
        }
    }

    public async Task<Result> SaveVttContentAsync(
        string relativePath,
        string vttContent,
        string referenceId,
        string referenceType,
        StorageCategory category = StorageCategory.Public,
        CancellationToken cancellationToken = default)
    {
        const string vttMimeType = "text/vtt";

        PresignedUrlResponse uploadResponse = _storageService.GenerateUploadUrlAsync(
            category,
            relativePath,
            referenceId,
            referenceType);

        string uploadUrl = uploadResponse.Url;

        using var content = new StringContent(vttContent, Encoding.UTF8, vttMimeType);
        using HttpClient client = _httpClientFactory.CreateClient();

        try
        {
            using HttpResponseMessage response = await client.PutAsync(
                new Uri(uploadUrl),
                content,
                cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                return Result.Failure(Error.Failure(
                    "Transcript.UploadFailed",
                    $"Failed to upload transcript: {response.StatusCode}"));
            }

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(Error.Failure(
                "Transcript.Exception",
                $"An error occurred while calling storage service: {ex.Message}"));
        }
    }
}
