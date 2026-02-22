using CoursePlatform.Contracts.StorageEvent;
using Courses.Application.Abstractions.Data;
using Courses.Application.Abstractions.Storage;
using Courses.Domain.Lessons;
using Courses.Domain.MediaProcessingTask;
using Courses.Domain.Shared.Primitives;
using Kernel.EventBus;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Lessons.EventHandlers;

internal sealed class LessonSubmitPostProductionDomainEventHandler
    : IDomainEventHandler<LessonSubmitPostProductionDomainEvent>
{
    private readonly IWriteDbContext _dbContext;

    public LessonSubmitPostProductionDomainEventHandler(
        IWriteDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task HandleAsync(
        LessonSubmitPostProductionDomainEvent message,
        CancellationToken cancellationToken = default)
    {
        var task = MediaProcessingTask.Create(message.LessonId, message.Message, message.RawResources);

        _dbContext.MediaProcessingTasks.Add(task);
    }
}
