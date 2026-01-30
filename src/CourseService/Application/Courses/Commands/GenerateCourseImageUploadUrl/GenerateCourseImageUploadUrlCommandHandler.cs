using Courses.Application.Abstractions.Storage;
using Courses.Application.Shared.Dtos;
using Courses.Domain.Abstractions.Repositories;
using Courses.Domain.Courses;
using Courses.Domain.Courses.Errors;
using Courses.Domain.Courses.Primitives;
using Courses.Domain.Shared.Primitives;
using Kernel;
using Kernel.Auth.Abstractions;
using Kernel.Auth.AuthTypes;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Courses.Commands.GenerateCourseImageUploadUrl;

internal sealed class GenerateCourseImageUploadUrlCommandHandler
    : ICommandHandler<GenerateCourseImageUploadUrlCommand, GenerateUploadUrlDto>
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

    public async Task<Result<GenerateUploadUrlDto>> Handle(
        GenerateCourseImageUploadUrlCommand request,
        CancellationToken cancellationToken = default)
    {
        Course? course = await _courseRepository.GetByIdAsync(request.Id, cancellationToken);

        if (course is null)
        {
            return Result.Failure<GenerateUploadUrlDto>(CourseErrors.NotFound);
        }

        var resourceId = ResourceId.Create(course.Id.ToString());
        bool hasPermission = _userContext.HasPermission(ActionType.Update, ResourceType.Course, resourceId);
        var currentUser = new UserId(_userContext.Id ?? Guid.Empty);

        if (course.InstructorId != currentUser && !hasPermission)
        {
            return Result.Failure<GenerateUploadUrlDto>(CourseErrors.Unauthorized);
        }

        string extension = Path.GetExtension(request.FileName).ToLowerInvariant();
        string uniqueFileName = $"{Guid.NewGuid()}{extension}";
        string rawFileKey = $"courses/{course.Id.Value}/images/{uniqueFileName}";

        Result<ImageUrl> imageUrlResult = ImageUrl.Create(rawFileKey);

        if (imageUrlResult.IsFailure)
        {
            return Result.Failure<GenerateUploadUrlDto>(imageUrlResult.Error);
        }

        string validatedFileKey = imageUrlResult.Value.Path;

        PresignedUrlResponse result = _storageService.GenerateUploadUrlAsync(
            StorageCategory.Public,
            validatedFileKey,
            course.Id.Value.ToString(),
            "courseimage",
            TimeSpan.FromMinutes(10)
        );

        var response = new GenerateUploadUrlDto(
            result.Url,
            result.FileKey,
            result.ExpiresAt
        );

        return Result.Success(response);
    }
}
