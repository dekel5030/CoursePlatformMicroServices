using Courses.Application.Abstractions.Storage;
using Courses.Application.Shared.Dtos;
using Courses.Domain.Abstractions.Repositories;
using Courses.Domain.Lessons;
using Courses.Domain.Lessons.Errors;
using Courses.Domain.Modules;
using Courses.Domain.Shared.Primitives;
using Kernel;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Lessons.Commands.GenerateLessonVideoUploadUrl;

internal sealed class GenerateLessonVideoUploadUrlCommandHandler
    : ICommandHandler<GenerateLessonVideoUploadUrlCommand, GenerateUploadUrlDto>
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

    public async Task<Result<GenerateUploadUrlDto>> Handle(
        GenerateLessonVideoUploadUrlCommand request,
        CancellationToken cancellationToken = default)
    {
        Lesson? lesson = await _lessonRepository.GetByIdAsync(request.LessonId, cancellationToken);

        if (lesson is null)
        {
            return Result<GenerateUploadUrlDto>.Failure(LessonErrors.NotFound);
        }

        string extension = Path.GetExtension(request.FileName).ToLowerInvariant();
        string uniqueFileName = $"{Guid.NewGuid()}{extension}";
        string rawFileKey = $"lessons/{lesson.Id}/video/{uniqueFileName}";

        Result<VideoUrl> imageUrlResult = VideoUrl.Create(rawFileKey);

        if (imageUrlResult.IsFailure)
        {
            return Result.Failure<GenerateUploadUrlDto>(imageUrlResult.Error);
        }

        string validatedFileKey = imageUrlResult.Value.Path;

        PresignedUrlResponse uploadUrl = _storageService.GenerateUploadUrlAsync(
            StorageCategory.Public,
            validatedFileKey,
            lesson.Id.ToString(),
            "lessonvideo",
            TimeSpan.FromHours(1));

        var response = new GenerateUploadUrlDto(uploadUrl.Url, validatedFileKey, uploadUrl.ExpiresAt);

        return Result.Success(response);
    }
}
