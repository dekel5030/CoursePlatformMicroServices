namespace Courses.Application.Shared.Dtos;

public record UploadUrlDto(
    string UploadUrl,
    string FileKey,
    DateTime ExpiresAt
);
