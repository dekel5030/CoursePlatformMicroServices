using Courses.Domain.Courses.Primitives;
using Courses.Domain.Lessons.Primitives;
using Courses.Domain.Modules.Primitives;
using Courses.Domain.Shared;
using Courses.Domain.Shared.Primitives;
using Kernel;

namespace Courses.Domain.Lessons;

public class Lesson : Entity<LessonId>
{
    public override LessonId Id { get; protected set; }
    public ModuleId ModuleId { get; private set; }
    public CourseId CourseId { get; private set; }

    public Title Title { get; private set; } = Title.Empty;
    public Description Description { get; private set; } = Description.Empty;
    public LessonAccess Access { get; private set; } = LessonAccess.Private;
    public LessonStatus Status { get; private set; } = LessonStatus.Raw;
    public int Index { get; private set; }
    public TimeSpan Duration { get; private set; } = TimeSpan.Zero;

    public ImageUrl? ThumbnailImageUrl { get; private set; }
    public VideoUrl? VideoUrl { get; private set; }
    public Url? Transcript { get; private set; }

    public Slug Slug { get; private set; }

    public IReadOnlyList<Attachment> Attachments => _attachments.AsReadOnly();
    public IReadOnlyList<TranscriptLine> TranscriptLines => _transcriptLines.AsReadOnly();
    public IReadOnlyList<Url> RawResources => _rawResources.AsReadOnly();

    private readonly List<Attachment> _attachments = [];
    private readonly List<TranscriptLine> _transcriptLines = [];
    private readonly List<Url> _rawResources = [];

    #pragma warning disable S1133
    #pragma warning disable CS8618
    [Obsolete("This constructor is for EF Core only.", error: true)]
    private Lesson() { }
    #pragma warning restore CS8618
    #pragma warning restore S1133

    private Lesson(
        ModuleId moduleId,
        CourseId courseId,
        LessonId id,
        Slug slug)
    {
        ModuleId = moduleId;
        CourseId = courseId;
        Id = id;
        Slug = slug;
    }

    internal static Result<Lesson> Create(
        CourseId courseId,
        ModuleId moduleId,
        Title? title,
        Description? description,
        int index = 0)
    { 
        var lessonId = LessonId.CreateNew();
        var slug = new Slug(lessonId.ToString());
        var lesson = new Lesson(moduleId, courseId, lessonId, slug)
        {
            Title = title ?? Title.Empty,
            Description = description ?? Description.Empty,
            Index = index,
        };

        lesson.Raise(new LessonCreatedDomainEvent(
            lesson.Id,
            lesson.ModuleId,
            lesson.CourseId,
            lesson.Slug,
            lesson.Title,
            lesson.Description,
            lesson.Access,
            lesson.Duration,
            lesson.Index,
            lesson.VideoUrl,
            lesson.ThumbnailImageUrl,
            lesson.Transcript));

        return Result.Success(lesson);
    }

    public Result UpdateMetadata(Title title, Description description, Slug slug)
    {
        if (Title == title && Description == description && Slug == slug)
        {
            return Result.Success();
        }

        Title = title;
        Description = description;
        Slug = slug;

        Raise(new LessonMetadataChangedDomainEvent(Id, ModuleId, CourseId, Title, Description, Slug));
        return Result.Success();
    }

    public Result UpdateTranscript(Url? transcriptUrl, string? vttContent = null)
    {
        if (Transcript == transcriptUrl && vttContent == null)
        {
            return Result.Success();
        }

        Transcript = transcriptUrl;
        if (!string.IsNullOrEmpty(vttContent))
        {
            _transcriptLines.Clear();
            _transcriptLines.AddRange(VttParser.Parse(vttContent));
        }

        Raise(new LessonTranscriptChangedDomainEvent(Id, ModuleId, CourseId, Transcript));
        return Result.Success();
    }

    public Result ChangeAccess(LessonAccess newAccess)
    {
        if (Access == newAccess)
        {
            return Result.Success();
        }

        Access = newAccess;
        Raise(new LessonAccessChangedDomainEvent(Id, ModuleId, CourseId, Access));
        return Result.Success();
    }

    internal void ChangeIndex(int newIndex)
    {
        if (Index == newIndex)
        {
            return;
        }

        Index = newIndex;
        Raise(new LessonIndexChangedDomainEvent(Id, ModuleId, CourseId, Index));
    }

    internal Result MoveToModule(ModuleId newModuleId, int newIndex)
    {
        if (ModuleId == newModuleId && Index == newIndex)
        {
            return Result.Success();
        }

        ModuleId previousModuleId = ModuleId;
        ModuleId = newModuleId;
        Index = newIndex;
        Raise(new LessonMovedDomainEvent(Id, previousModuleId, newModuleId, CourseId, Index));
        return Result.Success();
    }

    public Result UpdateMedia(VideoUrl? videoUrl, ImageUrl? thumbnailUrl, TimeSpan duration)
    {
        if (VideoUrl == videoUrl && ThumbnailImageUrl == thumbnailUrl && Duration == duration)
        {
            return Result.Success();
        }

        VideoUrl = videoUrl;
        ThumbnailImageUrl = thumbnailUrl;
        Duration = duration;

        Raise(new LessonMediaChangedDomainEvent(Id, ModuleId, CourseId, VideoUrl, ThumbnailImageUrl, Duration));
        return Result.Success();
    }

    public void Delete()
    {
        Raise(new LessonDeletedDomainEvent(Id, ModuleId, CourseId));
    }

    public Result SendToMediaProcessing(string message)
    {
        Status = LessonStatus.WaitingForMediaProcessing;
        Raise(new LessonSentToMediaProcessingDomainEvent(Id, CourseId, message, RawResources));

        return Result.Success();
    }
}
