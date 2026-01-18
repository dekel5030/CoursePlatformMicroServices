using CoursePlatform.Contracts.StorageEvent;
using Courses.Application.Abstractions.Data;
using Courses.Domain.Courses;
using Courses.Domain.Courses.Primitives;
using Courses.Domain.Lessons;
using Courses.Domain.Lessons.Primitives;
using Courses.Domain.Shared.Primitives;
using Kernel.EventBus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Courses.Application.EventConsumers;

internal sealed class FileUploadedEventConsumer : IEventConsumer<FileUploadedEvent>
{
    private readonly IWriteDbContext _writeDbContext;
    private readonly ILogger<FileUploadedEventConsumer> _logger;
    private const string CourseServiceName = "courseservice";
    private const string LessonImage = "lessonimage";
    private const string CourseImage = "courseimage";
    private const string LessonVideo = "lessonvideo";

    public FileUploadedEventConsumer(IWriteDbContext writeDbContext, ILogger<FileUploadedEventConsumer> logger)
    {
        _writeDbContext = writeDbContext;
        _logger = logger;
    }

    public async Task HandleAsync(FileUploadedEvent message, CancellationToken cancellationToken = default)
    {
        if (!message.OwnerService.Equals(CourseServiceName, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        switch (message.ReferenceType.ToLowerInvariant())
        {
            case CourseImage:
                await HandleCourseImageAsync(message, cancellationToken);
                break;

            case LessonImage:
                await HandleLessonImageAsync(message, cancellationToken);
                break;

            case LessonVideo:
                await HandleLessonVideoAsync(message, cancellationToken);
                break;
        }
    }

    private async Task HandleLessonVideoAsync(FileUploadedEvent message, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(message.ReferenceId, out Guid guidId))
        {
            _logger.LogError("Invalid ReferenceId format: {ReferenceId}", message.ReferenceId);
            return;
        }

        var lessonId = new LessonId(guidId);

        Lesson? lesson = await _writeDbContext.Lessons
            .FirstOrDefaultAsync(c => c.Id == lessonId, cancellationToken);

        if (lesson is null)
        {
            _logger.LogWarning("Lesson with ID {LessonId} not found for uploaded image", lessonId);
            return;
        }

        var videoUrl = new VideoUrl(message.FileKey);
        lesson.UpdateVideoData(videoUrl, TimeSpan.FromMinutes(8));

        await _writeDbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Updated image for lesson {LessonId}", lessonId);
    }

    private async Task HandleCourseImageAsync(
        FileUploadedEvent message,
        CancellationToken cancellationToken = default)
    {
        if (!Guid.TryParse(message.ReferenceId, out Guid guidId))
        {
            _logger.LogError("Invalid ReferenceId format: {ReferenceId}", message.ReferenceId);
            return;
        }

        var courseId = new CourseId(guidId);

        Course? course = await _writeDbContext.Courses
            .FirstOrDefaultAsync(c => c.Id == courseId, cancellationToken);

        if (course is null)
        {
            _logger.LogWarning("Course with ID {CourseId} not found for uploaded image", courseId);
            return;
        }

        var imageUrl = new ImageUrl(message.FileKey);
        course.AddImage(imageUrl, TimeProvider.System);

        await _writeDbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Updated image for course {CourseId}", courseId);
    }

    private async Task HandleLessonImageAsync(
        FileUploadedEvent message,
        CancellationToken cancellationToken = default)
    {
        if (!Guid.TryParse(message.ReferenceId, out Guid guidId))
        {
            _logger.LogError("Invalid ReferenceId format: {ReferenceId}", message.ReferenceId);
            return;
        }

        var lessonId = new LessonId(guidId);

        Lesson? lesson = await _writeDbContext.Lessons
            .FirstOrDefaultAsync(c => c.Id == lessonId, cancellationToken);

        if (lesson is null)
        {
            _logger.LogWarning("Lesson with ID {LessonId} not found for uploaded image", lessonId);
            return;
        }

        var imageUrl = new ImageUrl(message.FileKey);
        lesson.SetThumbnailImage(imageUrl);

        await _writeDbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Updated image for lesson {LessonId}", lessonId);
    }
}
