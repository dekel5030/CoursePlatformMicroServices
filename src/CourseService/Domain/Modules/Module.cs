using Courses.Domain.Courses.Primitives;
using Courses.Domain.Lessons;
using Courses.Domain.Lessons.Errors;
using Courses.Domain.Lessons.Primitives;
using Courses.Domain.Modules.Primitives;
using Courses.Domain.Shared;
using Courses.Domain.Shared.Primitives;
using Kernel;

namespace Courses.Domain.Modules;

public class Module : Entity<ModuleId>
{
    public override ModuleId Id { get; protected set; }
    public Title Title { get; private set; } = Title.Empty;
    public int Index { get; private set; }
    public CourseId CourseId { get; private set; }

#pragma warning disable S1133
#pragma warning disable CS8618
    [Obsolete("This constructor is for EF Core only.", error: true)]
    private Module() { }
#pragma warning restore CS8618
#pragma warning restore S1133

    private Module(CourseId courseId, int index, Title title)
    {
        Id = ModuleId.CreateNew();
        CourseId = courseId;
        Index = index;
        Title = title;
    }

    internal static Result<Module> Create(CourseId courseId, int index, Title? title = null)
    {
        var module = new Module(courseId, index, title ?? Title.Empty);

        module.Raise(new ModuleCreatedDomainEvent(module.Id, module.CourseId, module.Title, module.Index));
        return Result.Success(module);
    }

    public Result UpdateTitle(Title newTitle)
    {
        if (Title == newTitle)
        {
            return Result.Success();
        }

        Title = newTitle;
        Raise(new ModuleTitleChangedDomainEvent(Id, CourseId, Title));
        return Result.Success();
    }

    internal Result UpdateIndex(int newIndex)
    {
        if (Index == newIndex)
        {
            return Result.Success();
        }

        Index = newIndex;
        Raise(new ModuleIndexUpdatedDomainEvent(Id, CourseId, Index));
        return Result.Success();
    }

    public void Delete()
    {
        Raise(new ModuleDeletedDomainEvent(Id, CourseId));
    }
}
