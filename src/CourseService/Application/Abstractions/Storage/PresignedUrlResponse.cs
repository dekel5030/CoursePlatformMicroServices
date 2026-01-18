namespace Courses.Application.Abstractions.Storage;

public record PresignedUrlResponse(string Url, string FileKey, DateTime ExpiresAt);
