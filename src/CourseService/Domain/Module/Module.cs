using System;
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

    public static Result<Module> Create(CourseId courseId, int index, Title? title = null)
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

    public Result UpdateIndex(int newIndex)
    {
        if (Index == newIndex)
        {
            return Result.Success();
        }

        Index = newIndex;
        Raise(new ModuleIndexUpdatedDomainEvent(Id, CourseId, Index));
        return Result.Success();
    }

    public Result AddLesson(Title? title, Description? description)
    {
        int index = _lessons.Count;
        Result<Lesson> lessonResult = Lesson.Create(CourseId, Id, title, description, index);

        if (lessonResult.IsFailure)
        {
            return Result.Failure(lessonResult.Error);
        }

        _lessons.Add(lessonResult.Value);

        return Result.Success();
    }

    public Result UpdateLesson(
        LessonId lessonId,
        Title? title = null,
        Description? description = null,
        LessonAccess? access = null,
        int? index = null,
        Slug? slug = null)
    {
        Lesson? lesson = _lessons.FirstOrDefault(l => l.Id == lessonId);
        if (lesson is null)
        {
            return Result.Failure(LessonErrors.NotFound);
        }

        lesson.UpdateMetadata(title ?? lesson.Title, description ?? lesson.Description, slug ?? lesson.Slug);
        lesson.ChangeAccess(access ?? lesson.Access);
        lesson.ChangeIndex(index ?? lesson.Index);
        return Result.Success();

    }

    public Result UpdateLessonMedia(
        LessonId lessonId,
        ImageUrl? thumbnailImageUrl = null,
        VideoUrl? videoUrl = null,
        Url? transcriptUrl = null,
        string? transcript = null,
        TimeSpan? duration = null)
    {
        Lesson? lesson = _lessons.FirstOrDefault(l => l.Id == lessonId);

        if (lesson is null)
        {
            return Result.Failure(LessonErrors.NotFound);
        }

        lesson.UpdateMedia(videoUrl, thumbnailImageUrl, duration ?? lesson.Duration);
        lesson.UpdateTranscript(transcriptUrl, transcript);

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

        for (int i = 0; i < _lessons.Count; i++)
        {
            _lessons[i].ChangeIndex(i);
        }

        return Result.Success();
    }

    public void Delete()
    {
        Raise(new ModuleDeletedDomainEvent(Id, CourseId));
    }
}
