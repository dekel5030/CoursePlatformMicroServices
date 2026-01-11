using CoursePlatform.Contracts.StorageEvent;
using CoursePlatform.ServiceDefaults.Endpoints;
using Kernel;
using Kernel.EventBus;
using Microsoft.AspNetCore.Mvc;
using StorageService.Abstractions;

namespace StorageService.Endpoints;

internal sealed class Upload : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("storage/upload/{ownerService}/{bucket}/{referenceId}/{referenceType}/{*key}", async (
            string key,
            string ownerService,
            string referenceId,
            string bucket,
            string referenceType,
            HttpRequest request,
            IStorageProvider storage,
            IEventBus bus) =>
        {
            if (!request.HasFormContentType && request.ContentLength == 0)
                return Results.BadRequest("No file content.");

            var contentType = request.ContentType ?? "application/octet-stream";
            var contentLength = request.ContentLength ?? throw new ArgumentException("Content-Length is required");

            Result<string> fileResult = await storage.UploadObjectAsync(
                request.Body,
                key,
                contentType,
                contentLength,
                bucket);

            if (fileResult.IsFailure)
            {
                return Results.Problem("Failed to upload file to storage.");
            }

            await bus.PublishAsync(new FileUploadedEvent
            {
                FileKey = fileResult.Value,
                Bucket = bucket,
                ContentType = contentType,
                FileSize = contentLength,
                OwnerService = ownerService,
                ReferenceId = referenceId,
                ReferenceType = referenceType,
            });

            return Results.Ok(new { Key = fileResult.Value });
        })
        .WithMetadata(new DisableRequestSizeLimitAttribute());
    }
}