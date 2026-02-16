using Courses.Domain.Lessons;
using Courses.Domain.Lessons.Primitives;
using Kernel;

namespace Courses.Application.Abstractions.Storage;

public interface ITranscriptFileService
{
    Task<string?> GetVttContentAsync(
        string relativePath,
        StorageCategory storageCategory,
        CancellationToken cancellationToken = default);

    Task<Result> SaveVttContentAsync(
        string relativePath,
        string vttContent,
        string referenceId,
        string referenceType,
        StorageCategory category = StorageCategory.Public,
        CancellationToken cancellationToken = default);
}
