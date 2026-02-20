namespace CoursePlatform.Contracts.StorageEvent;

public sealed record FileUploadedEvent
{
    public required string FileKey { get; init; }
    public required string Bucket { get; init; }
    public required string ContentType { get; init; }
    public long FileSize { get; init; }

    public required string OwnerService { get; init; }
    public required string ReferenceId { get; init; }
    public required string ReferenceType { get; init; }
    public Dictionary<string, string> Metadata { get; init; } = new();

}

public sealed record VideoProcessingRequestEvent(
    string FileKey,
    string Bucket,
    string ReferenceId,
    string ReferenceType,
    string OwnerService);

public sealed record VideoProcessingCompletedEvent(
    string ReferenceId,
    string ReferenceType,
    string OwnerService,
    string MasterFileKey,
    double DurationSeconds,
    string? TranscriptKey,
    string Bucket);
