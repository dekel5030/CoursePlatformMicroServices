using Courses.Domain.Courses;
using Courses.Domain.Courses.Primitives;
using Courses.Domain.Lessons;
using Courses.Domain.Lessons.Errors;
using Courses.Domain.Lessons.Primitives;
using Courses.Domain.Module.Primitives;
using Courses.Domain.Shared;
using Courses.Domain.Shared.Primitives;
using Kernel;

namespace Courses.Domain.Module;

public class Module : Entity<ModuleId>
{
    public override ModuleId Id { get; protected set; }
    public Title Title { get; private set; } = Title.Empty;
    public int Index { get; private set; }

    public CourseId CourseId { get; private set; }
    public IReadOnlyList<Lesson> Lessons => _lessons.AsReadOnly();

    private readonly List<Lesson> _lessons = new();

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
        return Result.Success(module);
    }

    public Result AddLesson(Title? title, Description? description)
    {
        int index = _lessons.Count;

        Result<Lesson> lessonResult = Lesson.Create(title, description, index);
        if (lessonResult.IsFailure)
        {
            return Result.Failure(lessonResult.Error);
        }

        _lessons.Add(lessonResult.Value);
        return Result.Success();
    }

    public Result UpdateLesson(
        LessonId lessonId,
        Title? title,
        Description? description,
        LessonAccess? access,
        int? index)
    {
        Lesson? lesson = _lessons.FirstOrDefault(l => l.Id == lessonId);
        if (lesson is null)
        {
            return Result.Failure(LessonErrors.NotFound);
        }

        lesson.UpdateDetails(title, description, access, index);
        return Result.Success();
    }

    public Result RemoveLesson(LessonId lessonId)
    {
        Lesson? lesson = _lessons.FirstOrDefault(l => l.Id == lessonId);
        if (lesson is null)
        {
            return Result.Failure(LessonErrors.NotFound);
        }

        _lessons.Remove(lesson);
        return Result.Success();
    }
}
