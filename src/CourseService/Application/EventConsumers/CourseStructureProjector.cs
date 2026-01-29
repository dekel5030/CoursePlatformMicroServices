using CoursePlatform.Contracts.CourseService;
using Courses.Application.Abstractions.Data;
using Courses.Application.Abstractions.Data.ReadModels;
using Courses.Domain.Lessons.Primitives;
using Kernel.EventBus;

namespace Courses.Application.EventConsumers;

internal sealed class CourseStructureProjector :
    IEventConsumer<CourseCreatedIntegrationEvent>,
    IEventConsumer<ModuleCreatedIntegrationEvent>,
    IEventConsumer<ModuleTitleChangedIntegrationEvent>,
    IEventConsumer<ModuleIndexUpdatedIntegrationEvent>,
    IEventConsumer<ModuleDeletedIntegrationEvent>,
    IEventConsumer<LessonCreatedIntegrationEvent>,
    IEventConsumer<LessonMetadataChangedIntegrationEvent>,
    IEventConsumer<LessonMediaChangedIntegrationEvent>,
    IEventConsumer<LessonAccessChangedIntegrationEvent>,
    IEventConsumer<LessonIndexChangedIntegrationEvent>,
    IEventConsumer<LessonDeletedIntegrationEvent>
{
    private readonly IReadDbContext _readDbContext;

    public CourseStructureProjector(IReadDbContext readDbContext)
    {
        _readDbContext = readDbContext;
    }

    public async Task HandleAsync(
        CourseCreatedIntegrationEvent message,
        CancellationToken cancellationToken = default)
    {
        var structure = new CourseStructureReadModel
        {
            CourseId = message.CourseId,
            Modules = []
        };

        _readDbContext.CourseStructures.Add(structure);
        await _readDbContext.SaveChangesAsync(cancellationToken);
    }

    public Task HandleAsync(
        ModuleCreatedIntegrationEvent message,
        CancellationToken cancellationToken = default)
    {
        return UpdateStructureAsync(
            message.CourseId,
            structure => structure.AddModule(message.ModuleId, message.Title, message.Index),
            cancellationToken);
    }

    public Task HandleAsync(
        ModuleTitleChangedIntegrationEvent message,
        CancellationToken cancellationToken = default)
    {
        return UpdateStructureAsync(
            message.CourseId,
            structure => structure.UpdateModuleTitle(message.ModuleId, message.NewTitle),
            cancellationToken);
    }

    public Task HandleAsync(
        ModuleIndexUpdatedIntegrationEvent message,
        CancellationToken cancellationToken = default)
    {
        return UpdateStructureAsync(
            message.CourseId,
            structure => structure.UpdateModuleIndex(message.ModuleId, message.NewIndex),
            cancellationToken);
    }

    public Task HandleAsync(
        ModuleDeletedIntegrationEvent message,
        CancellationToken cancellationToken = default)
    {
        return UpdateStructureAsync(
            message.CourseId,
            structure => structure.RemoveModule(message.ModuleId),
            cancellationToken);
    }

    public Task HandleAsync(
        LessonCreatedIntegrationEvent message,
        CancellationToken cancellationToken = default)
    {
        return UpdateStructureAsync(
            message.CourseId,
            structure => structure.AddLesson(
                message.ModuleId,
                message.Id,
                message.Title,
                message.Index,
                message.Duration,
                message.ThumbnailUrl,
                Enum.Parse<LessonAccess>(message.Access)),
            cancellationToken);
    }

    public Task HandleAsync(
        LessonMetadataChangedIntegrationEvent message,
        CancellationToken cancellationToken = default)
    {
        return UpdateStructureAsync(
            message.CourseId,
            structure => structure.UpdateLessonMetadata(message.ModuleId, message.Id, message.Title),
            cancellationToken);
    }

    public Task HandleAsync(
        LessonMediaChangedIntegrationEvent message,
        CancellationToken cancellationToken = default)
    {
        return UpdateStructureAsync(
            message.CourseId,
            structure => structure.UpdateLessonMedia(message.ModuleId, message.Id, message.Duration, message.ThumbnailUrl),
            cancellationToken);
    }

    public Task HandleAsync(
        LessonAccessChangedIntegrationEvent message,
        CancellationToken cancellationToken = default)
    {
        return UpdateStructureAsync(
            message.CourseId,
            structure => structure.UpdateLessonAccess(message.ModuleId, message.Id, Enum.Parse<LessonAccess>(message.NewAccess)),
            cancellationToken);
    }

    public Task HandleAsync(
        LessonIndexChangedIntegrationEvent message,
        CancellationToken cancellationToken = default)
    {
        return UpdateStructureAsync(
            message.CourseId,
            structure => structure.UpdateLessonIndex(message.ModuleId, message.Id, message.NewIndex),
            cancellationToken);
    }

    public Task HandleAsync(
        LessonDeletedIntegrationEvent message,
        CancellationToken cancellationToken = default)
    {
        return UpdateStructureAsync(
            message.CourseId,
            structure => structure.RemoveLesson(message.ModuleId, message.Id),
            cancellationToken);
    }

    private async Task UpdateStructureAsync(
        Guid courseId,
        Action<CourseStructureReadModel> updateAction,
        CancellationToken cancellationToken)
    {
        CourseStructureReadModel? structure = await _readDbContext.CourseStructures
            .FindAsync([courseId], cancellationToken);

        if (structure is not null)
        {
            updateAction(structure);
            await _readDbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
