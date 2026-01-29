using Courses.Domain.Lessons.Primitives;

namespace Courses.Application.Abstractions.Data.ReadModels;

public sealed class CourseStructureReadModel
{
    public Guid CourseId { get; set; }
#pragma warning disable CA2227 // Collection properties should be read only
    public List<StructureModuleReadModel> Modules { get; set; } = [];
#pragma warning restore CA2227 // Collection properties should be read only

    public void AddModule(Guid moduleId, string title, int index)
    {
        if (Modules.Any(m => m.Id == moduleId))
        {
            return;
        }

        Modules.Add(new StructureModuleReadModel
        {
            Id = moduleId,
            Title = title,
            Index = index,
            Lessons = []
        });

        Modules = [.. Modules.OrderBy(m => m.Index)];
    }

    public void UpdateModuleTitle(Guid moduleId, string newTitle)
    {
        StructureModuleReadModel? module = Modules.Find(m => m.Id == moduleId);
        if (module is null)
        {
            return;
        }

        module.Title = newTitle;
    }

    public void UpdateModuleIndex(Guid moduleId, int newIndex)
    {
        StructureModuleReadModel? module = Modules.Find(m => m.Id == moduleId);
        if (module is not null)
        {
            module.Index = newIndex;
            Modules = [.. Modules.OrderBy(m => m.Index)];
        }
    }

    public void RemoveModule(Guid moduleId)
    {
        Modules.RemoveAll(m => m.Id == moduleId);
    }

    public void AddLesson(Guid moduleId, Guid lessonId, string title, int index, TimeSpan duration, string? thumbnailUrl, LessonAccess access)
    {
        StructureModuleReadModel? module = Modules.Find(m => m.Id == moduleId);
        if (module is null)
        {
            return;
        }

        if (module.Lessons.Any(l => l.Id == lessonId))
        {
            return;
        }

        module.Lessons.Add(new StructureLessonReadModel
        {
            Id = lessonId,
            ModuleId = moduleId,
            Title = title,
            Index = index,
            Duration = duration,
            ThumbnailUrl = thumbnailUrl,
            Access = access
        });

        module.Lessons = [.. module.Lessons.OrderBy(l => l.Index)];
    }

    public void UpdateLessonMetadata(Guid moduleId, Guid lessonId, string newTitle)
    {
        StructureLessonReadModel? lesson = Modules
            .Find(m => m.Id == moduleId)?
            .Lessons.Find(l => l.Id == lessonId);

        if (lesson is null)
        {
            return;
        }

        lesson.Title = newTitle;
    }

    public void UpdateLessonMedia(Guid moduleId, Guid lessonId, TimeSpan duration, string? thumbnailUrl)
    {
        StructureLessonReadModel? lesson = Modules
            .Find(m => m.Id == moduleId)?
            .Lessons.Find(l => l.Id == lessonId);

        if (lesson is not null)
        {
            lesson.Duration = duration;
            lesson.ThumbnailUrl = thumbnailUrl;
        }
    }

    public void UpdateLessonAccess(Guid moduleId, Guid lessonId, LessonAccess newAccess)
    {
        StructureLessonReadModel? lesson = Modules
            .Find(m => m.Id == moduleId)?
            .Lessons.Find(l => l.Id == lessonId);

        if (lesson is null)
        {
            return;
        }

        lesson.Access = newAccess;
    }

    public void UpdateLessonIndex(Guid moduleId, Guid lessonId, int newIndex)
    {
        StructureModuleReadModel? module = Modules.Find(m => m.Id == moduleId);
        StructureLessonReadModel? lesson = module?.Lessons.Find(l => l.Id == lessonId);

        if (lesson is not null && module is not null)
        {
            lesson.Index = newIndex;
            module.Lessons = [.. module.Lessons.OrderBy(l => l.Index)];
        }
    }

    public void RemoveLesson(Guid moduleId, Guid lessonId)
    {
        StructureModuleReadModel? module = Modules.Find(m => m.Id == moduleId);
        module?.Lessons.RemoveAll(l => l.Id == lessonId);
    }
}

public sealed class StructureModuleReadModel
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int Index { get; set; }
#pragma warning disable CA2227 // Collection properties should be read only
    public List<StructureLessonReadModel> Lessons { get; set; } = [];
#pragma warning restore CA2227 // Collection properties should be read only
}

public sealed class StructureLessonReadModel
{
    public Guid Id { get; set; }
    public Guid ModuleId { get; set; }
    public string Title { get; set; } = string.Empty;
    public int Index { get; set; }
    public LessonAccess Access { get; set; }
    public TimeSpan Duration { get; set; }
    public string? ThumbnailUrl { get; set; }
}
