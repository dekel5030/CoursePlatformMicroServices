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

namespace Courses.Application.Events;

internal class FileUploadedEventConsumer : IEventConsumer<FileUploadedEvent>
{
    private readonly IWriteDbContext _writeDbContext;
    private readonly ILogger<FileUploadedEventConsumer> _logger;
    private const string CourseServiceName = "courseservice";
    private const string LessonImage = "lessonimage";
    private const string CourseImage = "courseimage";

    public FileUploadedEventConsumer(IWriteDbContext writeDbContext, ILogger<FileUploadedEventConsumer> logger)
    {
        _writeDbContext = writeDbContext;
        _logger = logger;
    }

    public async Task HandleAsync(FileUploadedEvent @event, CancellationToken cancellationToken = default)
    {
        if (!@event.OwnerService.Equals(CourseServiceName, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        switch (@event.ReferenceType.ToLower())
        {
            case CourseImage:
                await HandleCourseImageAsync(@event, cancellationToken);
                break;

            case LessonImage:
                await HandleLessonImageAsync(@event, cancellationToken);
                break;
        }
    }

    private async Task HandleCourseImageAsync(
        FileUploadedEvent @event, 
        CancellationToken cancellationToken = default)
    {
        if (!CourseId.TryParse(@event.ReferenceId, out var courseId))
        {
            _logger.LogError("Invalid ReferenceId format: {ReferenceId}", @event.ReferenceId);
            return;
        }

        Course? course = await _writeDbContext.Courses
            .FirstOrDefaultAsync(c => c.Id == courseId, cancellationToken);

        if (course is null)
        {
            _logger.LogWarning("Course with ID {CourseId} not found for uploaded image", courseId);
            return;
        }

        var imageUrl = new ImageUrl(@event.FileKey);
        course.AddImage(imageUrl, TimeProvider.System);

        await _writeDbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Updated image for course {CourseId}", courseId);
    }

    private async Task HandleLessonImageAsync(
        FileUploadedEvent @event, 
        CancellationToken cancellationToken = default)
    {
        if (!LessonId.TryParse(@event.ReferenceId, out var lessonId))
        {
            _logger.LogError("Invalid ReferenceId format: {ReferenceId}", @event.ReferenceId);
            return;
        }

        Lesson? lesson = await _writeDbContext.Lessons
            .FirstOrDefaultAsync(c => c.Id == lessonId, cancellationToken);

        if (lesson is null)
        {
            _logger.LogWarning("Lesson with ID {LessonId} not found for uploaded image", lessonId);
            return;
        }

        var imageUrl = new ImageUrl(@event.FileKey);
        lesson.SetThumbnailImage(imageUrl);

        await _writeDbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Updated image for lesson {LessonId}", lessonId);
    }
}
