namespace StorageService.InternalContracts;

internal sealed record FileProcessingEvent
{
    public required string FileKey { get; init; }
    public required string Bucket { get; init; }
    public required string ContentType { get; init; }
    public required long FileSize { get; init; }
    public required string OwnerService { get; init; }
    public required string ReferenceId { get; init; }
    public required string ReferenceType { get; init; }
}

