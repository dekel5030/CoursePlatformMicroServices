using CoursePlatform.Contracts.StorageEvent;
using Courses.Application.Abstractions.Data;
using Courses.Domain.Lessons;
using Courses.Domain.Lessons.Primitives;
using Courses.Domain.Shared.Primitives;
using Kernel.EventBus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Courses.Application.EventConsumers;

internal sealed class VideoProcessingCompletedEventConsumer : IEventConsumer<VideoProcessingCompletedEvent>
{
    private const string CourseServiceName = "courseservice";
    private const string LessonVideoReferenceType = "lessonvideo";

    private readonly IWriteDbContext _dbContext;
    private readonly ILogger<VideoProcessingCompletedEventConsumer> _logger;

    public VideoProcessingCompletedEventConsumer(
        IWriteDbContext dbContext,
        ILogger<VideoProcessingCompletedEventConsumer> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task HandleAsync(
        VideoProcessingCompletedEvent message,
        CancellationToken cancellationToken = default)
    {
        if (!message.OwnerService.Equals(CourseServiceName, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        if (!message.ReferenceType.Equals(LessonVideoReferenceType, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        if (!Guid.TryParse(message.ReferenceId, out Guid guidId))
        {
            _logger.LogWarning("Invalid ReferenceId in VideoProcessingCompletedEvent: {ReferenceId}", message.ReferenceId);
            return;
        }

        var lessonId = new LessonId(guidId);

        Lesson? lesson = await _dbContext.Lessons
            .FirstOrDefaultAsync(l => l.Id == lessonId, cancellationToken);

        if (lesson is null)
        {
            _logger.LogWarning("Lesson {LessonId} not found for VideoProcessingCompletedEvent", lessonId);
            return;
        }

        var videoUrl = new VideoUrl(message.MasterFileKey);
        var duration = TimeSpan.FromSeconds(message.DurationSeconds);
        Url? transcript = string.IsNullOrEmpty(message.TranscriptKey)
            ? null
            : new Url(message.TranscriptKey);

        lesson.CompletePostProduction(videoUrl, duration, transcript);

        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Updated lesson {LessonId} with processed video {MasterFileKey}",
            lessonId,
            message.MasterFileKey);
    }
}
