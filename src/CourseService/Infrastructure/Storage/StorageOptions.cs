using Courses.Application.Abstractions.Storage;

namespace Courses.Infrastructure.Storage;

public class StorageOptions
{
    public const string SectionName = "Storage";
    public string BaseUrl { get; set; } = string.Empty;
    public Dictionary<StorageCategory, string> BucketMapping { get; } = new();
}
