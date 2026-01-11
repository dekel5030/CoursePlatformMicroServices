namespace StorageService.Abstractions;

internal sealed record PresignedUrlResponse(string Url, string Key, DateTime ExpiresAt);
