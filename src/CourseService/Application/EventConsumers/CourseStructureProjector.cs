using CoursePlatform.Contracts.CourseService;
using Courses.Application.Abstractions.Data;
using Courses.Application.Abstractions.Data.ReadModels;
using Courses.Domain.Lessons.Primitives;
using Kernel.EventBus;
using Microsoft.EntityFrameworkCore;

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
            Id = Guid.NewGuid(),
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
        return UpdateStructureAsync(message.CourseId, structure =>
        {
            structure.Modules.Add(new StructureModuleReadModel
            {
                Id = message.ModuleId,
                Title = message.Title,
                Index = message.Index,
                Lessons = []
            });
            structure.Modules = structure.Modules.OrderBy(m => m.Index).ToList();
        }, cancellationToken);
    }

    public Task HandleAsync(
        ModuleTitleChangedIntegrationEvent message,
        CancellationToken cancellationToken = default)
    {
        return UpdateStructureAsync(message.CourseId, structure =>
        {
            StructureModuleReadModel? module = structure.Modules.Find(m => m.Id == message.ModuleId);
            module?.Title = message.NewTitle;
        }, cancellationToken);
    }

    public Task HandleAsync(
        ModuleIndexUpdatedIntegrationEvent message,
        CancellationToken cancellationToken = default)
    {
        return UpdateStructureAsync(message.CourseId, structure =>
        {
            StructureModuleReadModel? module = structure.Modules.Find(m => m.Id == message.ModuleId);
            if (module is not null)
            {
                module.Index = message.NewIndex;
                structure.Modules = structure.Modules.OrderBy(m => m.Index).ToList();
            }
        }, cancellationToken);
    }

    public Task HandleAsync(
        ModuleDeletedIntegrationEvent message,
        CancellationToken cancellationToken = default)
    {
        return UpdateStructureAsync(message.CourseId, structure =>
        {
            structure.Modules.RemoveAll(m => m.Id == message.ModuleId);
        }, cancellationToken);
    }

    public Task HandleAsync(
        LessonCreatedIntegrationEvent message,
        CancellationToken cancellationToken = default)
    {
        return UpdateStructureAsync(message.CourseId, structure =>
        {
            StructureModuleReadModel? module = structure.Modules.Find(m => m.Id == message.ModuleId);
            if (module is null)
            {
                return;
            }

            module.Lessons.Add(new StructureLessonReadModel
            {
                Id = message.Id,
                ModuleId = message.ModuleId,
                Title = message.Title,
                Index = message.Index,
                Duration = message.Duration,
                ThumbnailUrl = message.ThumbnailUrl,
                Access = Enum.Parse<LessonAccess>(message.Access)
            });
            module.Lessons = module.Lessons.OrderBy(l => l.Index).ToList();
        }, cancellationToken);
    }

    public Task HandleAsync(
        LessonMetadataChangedIntegrationEvent message,
        CancellationToken cancellationToken = default)
    {
        return UpdateStructureAsync(message.CourseId, structure =>
        {
            StructureLessonReadModel? lesson = structure.Modules
                .Find(m => m.Id == message.ModuleId)?
                .Lessons.Find(l => l.Id == message.Id);

            lesson?.Title = message.Title;
        }, cancellationToken);
    }

    public Task HandleAsync(
        LessonMediaChangedIntegrationEvent message,
        CancellationToken cancellationToken = default)
    {
        return UpdateStructureAsync(message.CourseId, structure =>
        {
            StructureModuleReadModel? module = structure.Modules.Find(m => m.Id == message.ModuleId);
            StructureLessonReadModel? lesson = module?.Lessons.Find(l => l.Id == message.Id);

            if (lesson is not null)
            {
                lesson.Duration = message.Duration;
                lesson.ThumbnailUrl = message.ThumbnailUrl;
            }
        }, cancellationToken);
    }

    public Task HandleAsync(
        LessonAccessChangedIntegrationEvent message,
        CancellationToken cancellationToken = default)
    {
        return UpdateStructureAsync(message.CourseId, structure =>
        {
            StructureModuleReadModel? module = structure.Modules.FirstOrDefault(m => m.Id == message.ModuleId);
            StructureLessonReadModel? lesson = module?.Lessons.FirstOrDefault(l => l.Id == message.Id);

            lesson?.Access = Enum.Parse<LessonAccess>(message.NewAccess);
        }, cancellationToken);
    }

    public Task HandleAsync(
        LessonIndexChangedIntegrationEvent message,
        CancellationToken cancellationToken = default)
    {
        return UpdateStructureAsync(message.CourseId, structure =>
        {
            StructureModuleReadModel? module = structure.Modules.Find(m => m.Id == message.ModuleId);
            StructureLessonReadModel? lesson = module?.Lessons.Find(l => l.Id == message.Id);
            if (lesson is not null)
            {
                lesson.Index = message.NewIndex;
                module!.Lessons = module.Lessons.OrderBy(l => l.Index).ToList();
            }
        }, cancellationToken);
    }

    public Task HandleAsync(
        LessonDeletedIntegrationEvent message,
        CancellationToken cancellationToken = default)
    {
        return UpdateStructureAsync(message.CourseId, structure =>
        {
            StructureModuleReadModel? module = structure.Modules.FirstOrDefault(m => m.Id == message.ModuleId);
            module?.Lessons.RemoveAll(l => l.Id == message.Id);
        }, cancellationToken);
    }

    private async Task UpdateStructureAsync(
        Guid courseId,
        Action<CourseStructureReadModel> updateAction,
        CancellationToken cancellationToken)
    {
        CourseStructureReadModel? structure = await _readDbContext.CourseStructures
            .FirstOrDefaultAsync(s => s.CourseId == courseId, cancellationToken);

        if (structure is not null)
        {
            updateAction(structure);
            await _readDbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
