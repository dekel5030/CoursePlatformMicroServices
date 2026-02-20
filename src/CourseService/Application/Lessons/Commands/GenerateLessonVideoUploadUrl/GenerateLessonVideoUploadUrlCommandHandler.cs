using Courses.Application.Abstractions.Storage;
using Courses.Application.Shared.Dtos;
using Courses.Domain.Abstractions.Repositories;
using Courses.Domain.Lessons;
using Courses.Domain.Lessons.Errors;
using Courses.Domain.Shared.Primitives;
using Kernel;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Lessons.Commands.GenerateLessonVideoUploadUrl;

internal sealed class GenerateLessonVideoUploadUrlCommandHandler
    : ICommandHandler<GenerateLessonVideoUploadUrlCommand, UploadUrlDto>
{
    private readonly IObjectStorageService _storageService;
    private readonly ILessonRepository _lessonRepository;

    public GenerateLessonVideoUploadUrlCommandHandler(
        IObjectStorageService storageService,
        ILessonRepository lessonRepository)
    {
        _storageService = storageService;
        _lessonRepository = lessonRepository;
    }

    public async Task<Result<UploadUrlDto>> Handle(
    GenerateLessonVideoUploadUrlCommand request,
    CancellationToken cancellationToken = default)
    {
        Lesson? lesson = await _lessonRepository.GetByIdAsync(request.LessonId, cancellationToken);

        if (lesson is null)
        {
            return Result<UploadUrlDto>.Failure(LessonErrors.NotFound);
        }


        string extension = Path.GetExtension(request.FileName).ToLowerInvariant();
        string uniqueFileName = $"{Guid.NewGuid()}{extension}";

        string rawFileKey = $"lessons/{lesson.Id}/raw/{uniqueFileName}";

        var metadata = new Dictionary<string, string>
        {
            { "IsRaw", "true" },
            { "OriginalFileName", request.FileName }
        };

        PresignedUrlResponse uploadUrl = _storageService.GenerateUploadUrlAsync(
            StorageCategory.Public,
            rawFileKey,
            lesson.Id.ToString(),
            "lessonvideo",
            TimeSpan.FromHours(1),
            metadata);

        return Result.Success(new UploadUrlDto(uploadUrl.Url, rawFileKey, uploadUrl.ExpiresAt));
    }
}
