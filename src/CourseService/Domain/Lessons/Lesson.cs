using Courses.Domain.Lessons.Primitives;
using Courses.Domain.Module.Primitives;
using Courses.Domain.Shared;
using Courses.Domain.Shared.Primitives;
using Kernel;

namespace Courses.Domain.Lessons;

public class Lesson : Entity<LessonId>
{
    public override LessonId Id { get; protected set; }
    public ModuleId ModuleId { get; private set; }
    public Title Title { get; private set; } = Title.Empty;
    public Description Description { get; private set; } = Description.Empty;
    public LessonAccess Access { get; private set; } = LessonAccess.Private;
    public ImageUrl? ThumbnailImageUrl { get; private set; }
    public VideoUrl? VideoUrl { get; private set; }
    public Url? Transcript { get; private set; }
    public TimeSpan Duration { get; private set; } = TimeSpan.Zero;
    public Slug Slug { get; private set; }

    public int Index { get; private set; }

    public IReadOnlyList<Attachment> Attachments => _attachments.AsReadOnly();
    public IReadOnlyList<TranscriptLine> TranscriptLines => _transcriptLines.AsReadOnly();

    private readonly List<Attachment> _attachments = new();
    private List<TranscriptLine> _transcriptLines = new();

#pragma warning disable S1133
#pragma warning disable CS8618
    [Obsolete("This constructor is for EF Core only.", error: true)]
    private Lesson() { }
#pragma warning restore CS8618
#pragma warning restore S1133

    private Lesson(ModuleId moduleId, LessonId id, Slug slug)
    {
        ModuleId = moduleId;
        Id = id;
        Slug = slug;
    }

    internal static Result<Lesson> Create(
        ModuleId moduleId,
        Title? title,
        Description? description,
        int index = 0)
    {
        var lessonId = LessonId.CreateNew();
        var slug = new Slug(lessonId.ToString());
        var lesson = new Lesson(moduleId, lessonId, slug)
        {
            Title = title ?? Title.Empty,
            Description = description ?? Description.Empty,
            Index = index,
        };

        return Result.Success(lesson);
    }

    internal Result UpdateDetails(
        Title? title = null,
        Description? description = null,
        LessonAccess? access = null,
        int? index = null,
        Slug? slug = null)
    {
        if (title is not null && title != Title)
        {
            Title = title;
        }

        if (description is not null && description != Description)
        {
            Description = description;
        }

        if (access is not null && access != Access)
        {
            Access = access.Value;
        }

        if (index is not null && index != Index)
        {
            Index = index.Value;
        }

        if (slug is not null && slug != Slug)
        {
            Slug = slug;
        }

        return Result.Success();
    }

    internal Result UpdateMedia(
        ImageUrl? thumbnailImageUrl = null,
        VideoUrl? videoUrl = null,
        Url? transcriptUrl = null,
        TimeSpan? duration = null,
        string? transcript = null)
    {
        if (thumbnailImageUrl is not null && thumbnailImageUrl != ThumbnailImageUrl)
        {
            ThumbnailImageUrl = thumbnailImageUrl;
        }

        if (videoUrl is not null && videoUrl != VideoUrl)
        {
            VideoUrl = videoUrl;
        }

        if (transcriptUrl is not null && transcriptUrl != Transcript)
        {
            Transcript = transcriptUrl;
        }

        if (duration is not null && duration != Duration)
        {
            Duration = duration.Value;
        }

        if (transcript is not null)
        {
            _transcriptLines = VttParser.Parse(transcript);
        }

        return Result.Success();
    }

    internal void UpdateIndex(int index)
    {
        Index = index;
    }
}
