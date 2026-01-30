using CoursePlatform.Contracts.CourseService;
using Courses.Application.Abstractions.Data;
using Courses.Application.Abstractions.Data.ReadModels;
using Kernel.EventBus;
using Microsoft.EntityFrameworkCore;

namespace Courses.Application.EventConsumers;

/// <summary>
/// Projector for ModuleReadModel - maintains module projections.
/// Uses ONLY Integration Events (no WriteDbContext dependency).
/// </summary>
internal sealed class ModuleProjector :
    IEventConsumer<ModuleCreatedIntegrationEvent>,
    IEventConsumer<ModuleTitleChangedIntegrationEvent>,
    IEventConsumer<ModuleIndexUpdatedIntegrationEvent>,
    IEventConsumer<ModuleDeletedIntegrationEvent>,
    IEventConsumer<LessonCreatedIntegrationEvent>,
    IEventConsumer<LessonMediaChangedIntegrationEvent>,
    IEventConsumer<LessonDeletedIntegrationEvent>
{
    private readonly IReadDbContext _readDbContext;

    public ModuleProjector(IReadDbContext readDbContext)
    {
        _readDbContext = readDbContext;
    }

    public async Task HandleAsync(
        ModuleCreatedIntegrationEvent message,
        CancellationToken cancellationToken = default)
    {
        var module = new ModuleReadModel
        {
            Id = message.ModuleId,
            CourseId = message.CourseId,
            Title = message.Title,
            Index = message.Index,
            LessonCount = 0,
            TotalDurationSeconds = 0,
            CreatedAtUtc = DateTimeOffset.UtcNow,
            UpdatedAtUtc = DateTimeOffset.UtcNow
        };

        _readDbContext.Modules.Add(module);
        await _readDbContext.SaveChangesAsync(cancellationToken);
    }

    public Task HandleAsync(
        ModuleTitleChangedIntegrationEvent message,
        CancellationToken cancellationToken = default)
    {
        return UpdateModuleAsync(
            message.ModuleId,
            module => module.Title = message.NewTitle,
            cancellationToken);
    }

    public Task HandleAsync(
        ModuleIndexUpdatedIntegrationEvent message,
        CancellationToken cancellationToken = default)
    {
        return UpdateModuleAsync(
            message.ModuleId,
            module => module.Index = message.NewIndex,
            cancellationToken);
    }

    public async Task HandleAsync(
        ModuleDeletedIntegrationEvent message,
        CancellationToken cancellationToken = default)
    {
        ModuleReadModel? module = await _readDbContext.Modules
            .FirstOrDefaultAsync(m => m.Id == message.ModuleId, cancellationToken);

        if (module is not null)
        {
            _readDbContext.Modules.Remove(module);
            await _readDbContext.SaveChangesAsync(cancellationToken);
        }
    }

    // Stats Events

    public Task HandleAsync(
        LessonCreatedIntegrationEvent message,
        CancellationToken cancellationToken = default)
    {
        return UpdateModuleAsync(
            message.ModuleId,
            module =>
            {
                module.LessonCount++;
                module.TotalDurationSeconds += message.Duration.TotalSeconds;
            },
            cancellationToken);
    }

    public Task HandleAsync(
        LessonMediaChangedIntegrationEvent message,
        CancellationToken cancellationToken = default)
    {
        return UpdateModuleDurationAsync(message.ModuleId, message.Id, message.Duration, cancellationToken);
    }

    public Task HandleAsync(
        LessonDeletedIntegrationEvent message,
        CancellationToken cancellationToken = default)
    {
        return UpdateModuleAsync(
            message.ModuleId,
            module => module.LessonCount = Math.Max(0, module.LessonCount - 1),
            cancellationToken);
    }

    // Helpers

    private async Task UpdateModuleAsync(
        Guid moduleId,
        Action<ModuleReadModel> updateAction,
        CancellationToken cancellationToken)
    {
        ModuleReadModel? module = await _readDbContext.Modules
            .FirstOrDefaultAsync(m => m.Id == moduleId, cancellationToken);

        if (module is null)
        {
            return;
        }

        updateAction(module);
        module.UpdatedAtUtc = DateTimeOffset.UtcNow;

        await _readDbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task UpdateModuleDurationAsync(
        Guid moduleId,
        Guid lessonId,
        TimeSpan newDuration,
        CancellationToken cancellationToken)
    {
        ModuleReadModel? module = await _readDbContext.Modules
            .FirstOrDefaultAsync(m => m.Id == moduleId, cancellationToken);

        LessonReadModel? lesson = await _readDbContext.Lessons
            .FirstOrDefaultAsync(l => l.Id == lessonId, cancellationToken);

        if (module is null || lesson is null)
        {
            return;
        }

        double durationDiff = newDuration.TotalSeconds - lesson.Duration.TotalSeconds;
        module.TotalDurationSeconds += durationDiff;
        module.UpdatedAtUtc = DateTimeOffset.UtcNow;

        await _readDbContext.SaveChangesAsync(cancellationToken);
    }
}
