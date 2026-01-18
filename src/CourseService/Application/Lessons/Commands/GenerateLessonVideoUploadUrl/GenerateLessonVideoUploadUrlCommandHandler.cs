using Courses.Application.Abstractions.Repositories;
using Courses.Application.Abstractions.Storage;
using Courses.Application.Shared.Dtos;
using Courses.Domain.Courses;
using Courses.Domain.Courses.Errors;
using Courses.Domain.Lessons;
using Courses.Domain.Lessons.Errors;
using Courses.Domain.Shared.Primitives;
using Kernel;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Lessons.Commands.GenerateLessonVideoUploadUrl;

internal class GenerateLessonVideoUploadUrlCommandHandler
    : ICommandHandler<GenerateLessonVideoUploadUrlCommand, GenerateUploadUrlDto>
{
    private readonly IObjectStorageService _storageService;
    private readonly ICourseRepository _courseRepository;

    public GenerateLessonVideoUploadUrlCommandHandler(
        IObjectStorageService storageService,
        ICourseRepository courseRepository)
    {
        _storageService = storageService;
        _courseRepository = courseRepository;
    }

    public async Task<Result<GenerateUploadUrlDto>> Handle(
        GenerateLessonVideoUploadUrlCommand request,
        CancellationToken cancellationToken = default)
    {
        Course? course = await _courseRepository.GetByIdAsync(request.CourseId, cancellationToken);

        if (course is null)
        {
            return Result<GenerateUploadUrlDto>.Failure(CourseErrors.NotFound);
        }

        Lesson? lesson = course.Lessons.FirstOrDefault(l => l.Id == request.LessonId);

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
            lesson.Id,
            "lesson/video",
            TimeSpan.FromHours(1));

        var response = new GenerateUploadUrlDto(uploadUrl.FileKey, validatedFileKey, uploadUrl.ExpiresAt);

        return Result.Success(response);
    }
}
