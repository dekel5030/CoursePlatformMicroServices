namespace CoursePlatform.Contracts.StorageEvent;

public record FileUploadedEvent
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