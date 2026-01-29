using Courses.Domain.Lessons.Primitives;

namespace Courses.Application.Abstractions.Data.ReadModels;

public sealed class CourseStructureReadModel
{
    public Guid Id { get; set; }
    public Guid CourseId { get; set; }
#pragma warning disable CA2227 // Collection properties should be read only
    public List<StructureModuleReadModel> Modules { get; set; } = [];
#pragma warning restore CA2227 // Collection properties should be read only
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
