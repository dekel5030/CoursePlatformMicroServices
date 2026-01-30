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
    private readonly IModuleRepository _moduleRepository;

    public GenerateLessonVideoUploadUrlCommandHandler(
        IObjectStorageService storageService,
        IModuleRepository moduleRepository)
    {
        _storageService = storageService;
        _moduleRepository = moduleRepository;
    }

    public async Task<Result<GenerateUploadUrlDto>> Handle(
        GenerateLessonVideoUploadUrlCommand request,
        CancellationToken cancellationToken = default)
    {
        Module? module = await _moduleRepository.GetByIdAsync(request.ModuleId, cancellationToken);

        if (module is null)
        {
            return Result<GenerateUploadUrlDto>.Failure(
                Error.NotFound("Module.NotFound", "The specified module was not found."));
        }

        Lesson? lesson = module.Lessons.FirstOrDefault(l => l.Id == request.LessonId);

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
