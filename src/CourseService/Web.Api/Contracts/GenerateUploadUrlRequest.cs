namespace Courses.Api.Contracts;

internal record GenerateUploadUrlRequest(
    string FileName,   
    string ContentType
);

internal record GenerateUploadUrlResponse(
    string UploadUrl,
    string FileKey,
    DateTime ExpiresAt
);
