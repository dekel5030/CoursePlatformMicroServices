using CoursePlatform.Contracts.StorageEvent;
using CoursePlatform.ServiceDefaults.Endpoints;
    using Kernel;
    using Kernel.EventBus;
    using Microsoft.AspNetCore.Mvc;
    using StorageService.Abstractions;
    using StorageService.InternalContracts;

    namespace StorageService.Endpoints;

    internal sealed class Upload : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPut("storage/upload/{bucket}/{*key}", async (
                string key,
                string bucket,
                HttpRequest request,
                IStorageProvider storage,
                IEventBus bus) =>
            {
                var metadata = request.Headers
                    .Where(header => header.Key.StartsWith("x-meta-", StringComparison.OrdinalIgnoreCase))
                    .ToDictionary(
                        h => h.Key.Replace("x-meta-", "", StringComparison.OrdinalIgnoreCase),
                        h => h.Value.ToString()
                    );

                string? ownerService = request.Headers["x-owner-service"];
                string? referenceId = request.Headers["x-ref-id"];
                string? referenceType = request.Headers["x-ref-type"];

                if (string.IsNullOrEmpty(ownerService) || string.IsNullOrEmpty(referenceId))
                {
                    return Results.BadRequest("Missing mandatory metadata headers (owner, ref-id).");
                }

                // 2. ביצוע ההעלאה
                string contentType = request.ContentType ?? "application/octet-stream";
                long contentLength = request.ContentLength ?? throw new ArgumentException("Content-Length required");

                Result<string> fileResult = await storage.UploadObjectAsync(
                    request.Body,
                    key,
                    contentType,
                    contentLength,
                    bucket);

                if (fileResult.IsFailure)
                {
                    return Results.Problem("Upload failed.");
                }

                await bus.PublishAsync(new FileUploadedEvent
                {
                    FileKey = fileResult.Value,
                    Bucket = bucket,
                    Metadata = metadata,
                    OwnerService = ownerService,
                    ReferenceId = referenceId,
                    ReferenceType = referenceType ?? "",
                    FileSize = contentLength,
                    ContentType = contentType
                });

                return Results.Ok(new { Key = fileResult.Value });
            })
            .WithMetadata(new DisableRequestSizeLimitAttribute());
        }
    }

