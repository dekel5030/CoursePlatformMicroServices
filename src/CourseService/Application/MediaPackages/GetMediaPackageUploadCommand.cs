using Courses.Application.Abstractions.Data;
using Courses.Application.Abstractions.Storage;
using Courses.Application.Shared.Dtos;
using Courses.Domain.Abstractions.Repositories;
using Courses.Domain.CaptureSessions;
using Courses.Domain.Courses.Primitives;
using Kernel;
using Kernel.Auth.Abstractions;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.MediaPackages;

public record FileRequestDto(string FileName, TimeSpan Duration);

public sealed record GetMediaPackageUploadCommand(
    Guid CourseId,
    List<FileRequestDto> Files) : ICommand<IReadOnlyList<MediaPackageUploadResponse>>;

public sealed record MediaPackageUploadResponse(
    Guid MediaPackageId,
    Dictionary<string, UploadUrlDto> UploadUrls);

public sealed record UploadUrlDto(string FileName, string UploadUrl);

public sealed class GetMediaPackageBulkUploadCommandHandler
    : ICommandHandler<GetMediaPackageUploadCommand, IReadOnlyList<MediaPackageUploadResponse>>
{
    private readonly IObjectStorageService _storageService;
    private readonly IUserContext _userContext;

    public GetMediaPackageBulkUploadCommandHandler(
        IObjectStorageService storageService, 
        IUserContext userContext)
    {
        _storageService = storageService;
        _userContext = userContext;
    }

    public Task<Result<IReadOnlyList<MediaPackageUploadResponse>>> Handle(
        GetMediaPackageUploadCommand request, 
        CancellationToken cancellationToken = default)
    {
        List<List<FileRequestDto>> groups = GroupFilesByDuration(request.Files, TimeSpan.FromSeconds(1));

        var responsePackages = new List<MediaPackageUploadResponse>();

        foreach (List<FileRequestDto> group in groups)
        {
            Result<CaptureSession> mediaPackageResult = CaptureSession.Create(
                new CourseId(request.CourseId),
                _userContext.UserId,
                group.Count,
                $"Auto-grouped package (Duration: ~{group.First().Duration:mm\\:ss})");

            if (mediaPackageResult.IsFailure)
            {
                return mediaPackageResult.Error;
            }

            var mediaPackage = mediaPackageResult.Value;
            var uploadUrls = new List<UploadUrlDto>();

            // 3. יצירת לינקים לכל הקבצים בקבוצה
            foreach (var file in group)
            {
                // נתיב: courses/{id}/raw/{packageId}/{random}_{name}
                string fileKey = $"courses/{request.CourseId}/raw/{mediaPackage.Id.Value}/{Guid.NewGuid()}_{file.FileName}";

                string presignedUrl = await _storageService.GetUploadPreSignedUrlAsync(fileKey, TimeSpan.FromMinutes(60));

                uploadUrls.Add(new UploadUrlDto(
                    file.FileName,
                    presignedUrl,
                    fileKey,
                    DateTime.UtcNow.AddMinutes(60)));
            }

            // שמירה ב-Repository (בשלב זה Assets עדיין ריק, הם יתווספו מהאירוע של ה-Storage)
            await _repository.AddAsync(mediaPackage, cancellationToken);

            responsePackages.Add(new MediaPackageUploadResponse(mediaPackage.Id.Value, uploadUrls));
        }

        // 4. שמירה אטומית
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new BulkUploadResponse(responsePackages);
    }

    private static List<List<FileRequestDto>> GroupFilesByDuration(List<FileRequestDto> files, TimeSpan tolerance)
    {
        var sortedFiles = files.OrderBy(f => f.Duration).ToList();
        var groups = new List<List<FileRequestDto>>();
        if (sortedFiles.Count == 0)
        {
            return groups;
        }

        var currentGroup = new List<FileRequestDto> { sortedFiles[0] };
        groups.Add(currentGroup);

        for (int i = 1; i < sortedFiles.Count; i++)
        {
            FileRequestDto last = currentGroup[^1];
            TimeSpan diff = (sortedFiles[i].Duration - last.Duration).Duration();

            if (diff <= tolerance)
            {
                currentGroup.Add(sortedFiles[i]);
            }
            else
            {
                currentGroup = new List<FileRequestDto> { sortedFiles[i] };
                groups.Add(currentGroup);
            }
        }
        return groups;
    }
}
