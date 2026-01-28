using Courses.Domain.Courses.Primitives;
using Courses.Domain.Lessons;
using Courses.Domain.Module.Primitives;
using Courses.Domain.Shared.Primitives;

namespace Courses.Domain.Module;

public interface IModuleSnapshot
{     
    ModuleId Id { get; }
    Title Title { get; }
    int Index { get; }
    CourseId CourseId { get; }
    IReadOnlyList<Lesson> Lessons { get; }
}
