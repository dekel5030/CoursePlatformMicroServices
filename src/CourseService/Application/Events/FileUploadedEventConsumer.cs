using CoursePlatform.Contracts.StorageEvent;
using Courses.Application.Abstractions.Data;
using Courses.Domain.Courses;
using Courses.Domain.Courses.Primitives;
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
            case "courseimage":
                await HandleCourseImageAsync(@event, cancellationToken);
                break;

            case "coursevideo":
                break;
        }
    }

    private async Task HandleCourseImageAsync(FileUploadedEvent @event, CancellationToken ct)
    {
        if (!Guid.TryParse(@event.ReferenceId, out var guidId))
        {
            _logger.LogError("Invalid ReferenceId format: {ReferenceId}", @event.ReferenceId);
            return;
        }

        var courseId = new CourseId(guidId);
        Course? course = await _writeDbContext.Courses
            .FirstOrDefaultAsync(c => c.Id == courseId, ct);

        if (course is null)
        {
            _logger.LogWarning("Course with ID {CourseId} not found for uploaded image", courseId);
            return;
        }

        var imageUrl = new ImageUrl(@event.FileKey);
        course.AddImage(imageUrl, TimeProvider.System);

        await _writeDbContext.SaveChangesAsync(ct);

        _logger.LogInformation("Updated image for course {CourseId}", courseId);
    }
}
