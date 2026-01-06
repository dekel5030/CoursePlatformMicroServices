namespace StorageService.Abstractions;

public record PresignedUrlResponse(string Url, string Key, DateTime ExpiresAt);
