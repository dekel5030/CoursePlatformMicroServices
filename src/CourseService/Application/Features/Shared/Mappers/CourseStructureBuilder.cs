using Courses.Application.Features.Dtos;
using Courses.Domain.Lessons;
using Courses.Domain.Modules;

namespace Courses.Application.Features.Shared.Mappers;

public static class CourseStructureBuilder
{
    public static CourseStructureDto Build(
        IReadOnlyList<Module> modules,
        IReadOnlyList<Lesson> lessons)
    {
        var orderedModules = modules.OrderBy(m => m.Index).ToList();
        var moduleIds = orderedModules.Select(m => m.Id.Value).ToList();
        var moduleLessonIds = orderedModules.ToDictionary(
            m => m.Id.Value,
            m => (IReadOnlyList<Guid>)lessons
                .Where(l => l.ModuleId == m.Id)
                .OrderBy(l => l.Index)
                .Select(l => l.Id.Value)
                .ToList());

        return new CourseStructureDto
        {
            ModuleIds = moduleIds,
            ModuleLessonIds = moduleLessonIds
        };
    }
}
