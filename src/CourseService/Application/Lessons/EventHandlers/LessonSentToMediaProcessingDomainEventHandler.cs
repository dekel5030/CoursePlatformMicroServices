using Courses.Application.Abstractions.Data;
using Courses.Domain.Lessons;
using Courses.Domain.MediaProcessingTask;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Lessons.EventHandlers;

internal sealed class LessonSentToMediaProcessingDomainEventHandler
    : IDomainEventHandler<LessonSentToMediaProcessingDomainEvent>
{
    private readonly IWriteDbContext _dbContext;

    public LessonSentToMediaProcessingDomainEventHandler(IWriteDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task HandleAsync(
        LessonSentToMediaProcessingDomainEvent message, 
        CancellationToken cancellationToken = default)
    {
        var task = MediaProcessingTask.Create(message.LessonId, message.Message, message.RawResources);

        _dbContext.MediaProcessingTasks.Add(task);

        return Task.CompletedTask;
    }
}
