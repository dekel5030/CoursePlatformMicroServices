using Courses.Application.Abstractions.Data;
using Courses.Domain.Lessons;
using Courses.Domain.MediaProcessingTask.Events;
using Courses.Domain.Shared.Primitives;
using Kernel;
using Kernel.Messaging.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Courses.Application.Tasks.MediaProcessingTask.EventHandlers;

internal sealed class MediaProcessingTaskCompletedDomainEventHandler
    : IDomainEventHandler<MediaProcesingTaskCompletedDomainEvent>
{
    private readonly LessonManagementService _lessonManagementService;
    private readonly ILogger<MediaProcessingTaskCompletedDomainEventHandler> _logger;

    public MediaProcessingTaskCompletedDomainEventHandler(
        ILogger<MediaProcessingTaskCompletedDomainEventHandler> logger,
        LessonManagementService lessonManagementService)
    {
        _logger = logger;
        _lessonManagementService = lessonManagementService;
    }

    public async Task HandleAsync(
        MediaProcesingTaskCompletedDomainEvent message,
        CancellationToken cancellationToken = default)
    {
        Result<List<Lesson>> result = await _lessonManagementService.SplitLessonAsync(
            message.OriginalLessonId,
            message.OutputResources,
            cancellationToken);

        if (result.IsFailure)
        {
            _logger.LogError("Failed to split lesson {LessonId}: {Error}", message.OriginalLessonId, result.Error);
            throw new InvalidOperationException(result.Error.Description);
        }

        _logger.LogInformation("Successfully split lesson into {Count} parts", message.OutputResources.Count);
    }
}
