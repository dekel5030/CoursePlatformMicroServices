namespace Courses.Application.Abstractions.Storage;

public record ResolvedUrl(
    string Value,
    StorageCategory Category,
    DateTime? ExpiresAt = null
);