namespace Courses.Application.Shared.Dtos;

public record GenerateUploadUrlDto(
    string UploadUrl,
    string FileKey,
    DateTime ExpiresAt
);
