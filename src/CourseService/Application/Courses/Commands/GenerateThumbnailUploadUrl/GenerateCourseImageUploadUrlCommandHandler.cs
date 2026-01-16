using System;
using System.Collections.Generic;
using System.Text;
using Courses.Application.Abstractions.Repositories;
using Courses.Application.Abstractions.Storage;
using Courses.Domain.Courses;
using Courses.Domain.Courses.Errors;
using Courses.Domain.Courses.Primitives;
using Kernel;
using Kernel.Auth.Abstractions;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Courses.Commands.GenerateThumbnailUploadUrl;

internal sealed class GenerateCourseImageUploadUrlCommandHandler
    : ICommandHandler<GenerateCourseImageUploadUrlCommand, GenerateUploadUrlResponse>
{
    private readonly ICourseRepository _courseRepository;
    private readonly IUserContext _userContext;
    private readonly IObjectStorageService _storageService;

    public GenerateCourseImageUploadUrlCommandHandler(
        ICourseRepository courseRepository, 
        IUserContext userContext, 
        IObjectStorageService storageService)
    {
        _courseRepository = courseRepository;
        _userContext = userContext;
        _storageService = storageService;
    }

    public async Task<Result<GenerateUploadUrlResponse>> Handle(
        GenerateCourseImageUploadUrlCommand request,
        CancellationToken cancellationToken = default)
    {
        Course? course = await _courseRepository.GetByIdAsync(request.Id, cancellationToken);

        if (course is null)
        {
            return Result.Failure<GenerateUploadUrlResponse>(CourseErrors.NotFound);
        }

        var currentUser = new UserId(_userContext.Id ?? Guid.Empty);
        if (course.InstructorId != currentUser)
        {
            return Result.Failure<GenerateUploadUrlResponse>(CourseErrors.Unauthorized);
        }

        string extension = Path.GetExtension(request.FileName).ToLowerInvariant();
        if (string.IsNullOrEmpty(extension) || !IsAllowedImageExtension(extension))
        {
            return Result.Failure<GenerateUploadUrlResponse>(Error.Validation("Upload.InvalidFormat", "Invalid image format"));
        }

        string uniqueFileName = $"{Guid.NewGuid()}{extension}";
        string fileKey = $"courses/{course.Id.Value}/thumbnails/{uniqueFileName}";

        PresignedUrlResponse result = _storageService.GenerateUploadUrlAsync(
            StorageCategory.Public,
            fileKey,
            course.Id.Value.ToString(),
            "courseimage",
            TimeSpan.FromMinutes(10)
        );

        var response = new GenerateUploadUrlResponse(
            result.Url,
            result.FileKey,
            result.ExpiresAt
        );

        return Result.Success(response);
    }

    private static bool IsAllowedImageExtension(string ext)
        => new[] { ".jpg", ".jpeg", ".png", ".webp" }.Contains(ext);
}
