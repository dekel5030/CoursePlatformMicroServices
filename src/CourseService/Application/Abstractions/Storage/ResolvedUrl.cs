namespace Courses.Application.Abstractions.Storage;

public record ResolvedUrl(
    Uri Value,
    StorageCategory Category,
    DateTime? ExpiresAt = null
);