using Kernel;
using Users.Domain.Users.Primitives;

namespace Users.Domain.Users;

public class LecturerProfile : Entity
{
    public LecturerProfileId Id { get; private set; }
    public UserId UserId { get; private set; }
    public string? ProfessionalBio { get; private set; }
    public string? Expertise { get; private set; }
    public int YearsOfExperience { get; private set; }

    private readonly List<Project> _projects = [];
    public IReadOnlyCollection<Project> Projects => _projects.AsReadOnly();

    private readonly List<MediaItem> _mediaItems = [];
    public IReadOnlyCollection<MediaItem> MediaItems => _mediaItems.AsReadOnly();

    private readonly List<Post> _posts = [];
    public IReadOnlyCollection<Post> Posts => _posts.AsReadOnly();

#pragma warning disable CS8618
    private LecturerProfile() { }
#pragma warning restore CS8618

    public static Result<LecturerProfile> CreateProfile(
        UserId userId,
        string? professionalBio = null,
        string? expertise = null,
        int yearsOfExperience = 0)
    {
        var profile = new LecturerProfile
        {
            Id = LecturerProfileId.New(),
            UserId = userId,
            ProfessionalBio = professionalBio,
            Expertise = expertise,
            YearsOfExperience = yearsOfExperience
        };

        return Result.Success(profile);
    }

    public Result UpdateProfile(
        string? professionalBio = null,
        string? expertise = null,
        int? yearsOfExperience = null)
    {
        if (professionalBio is not null)
        {
            ProfessionalBio = professionalBio;
        }

        if (expertise is not null)
        {
            Expertise = expertise;
        }

        if (yearsOfExperience.HasValue)
        {
            YearsOfExperience = yearsOfExperience.Value;
        }

        return Result.Success();
    }

    public Result AddProject(Project project)
    {
        _projects.Add(project);
        return Result.Success();
    }

    public Result RemoveProject(Guid projectId)
    {
        Project? project = _projects.FirstOrDefault(p => p.Id == projectId);
        if (project is null)
        {
            return Result.Failure(Error.NotFound("LecturerProfile.ProjectNotFound", "Project not found"));
        }

        _projects.Remove(project);
        return Result.Success();
    }

    public Result AddMediaItem(MediaItem mediaItem)
    {
        _mediaItems.Add(mediaItem);
        return Result.Success();
    }

    public Result RemoveMediaItem(Guid mediaItemId)
    {
        MediaItem? mediaItem = _mediaItems.FirstOrDefault(m => m.Id == mediaItemId);
        if (mediaItem is null)
        {
            return Result.Failure(Error.NotFound("LecturerProfile.MediaItemNotFound", "Media item not found"));
        }

        _mediaItems.Remove(mediaItem);
        return Result.Success();
    }

    public Result AddPost(Post post)
    {
        _posts.Add(post);
        return Result.Success();
    }

    public Result RemovePost(Guid postId)
    {
        Post? post = _posts.FirstOrDefault(p => p.Id == postId);
        if (post is null)
        {
            return Result.Failure(Error.NotFound("LecturerProfile.PostNotFound", "Post not found"));
        }

        _posts.Remove(post);
        return Result.Success();
    }
}

public class Project
{
    public Guid Id { get; private set; }
    public string Title { get; private set; } = null!;
    public string? Description { get; private set; }
    public string? Url { get; private set; }
    public string? ThumbnailUrl { get; private set; }
    public DateTime CreatedAt { get; private set; }

#pragma warning disable CS8618
    private Project() { }
#pragma warning restore CS8618

    public static Project Create(
        string title,
        string? description = null,
        string? url = null,
        string? thumbnailUrl = null)
    {
        return new Project
        {
            Id = Guid.CreateVersion7(),
            Title = title,
            Description = description,
            Url = url,
            ThumbnailUrl = thumbnailUrl,
            CreatedAt = DateTime.UtcNow
        };
    }
}

public class MediaItem
{
    public Guid Id { get; private set; }
    public string Url { get; private set; } = null!;
    public string? Title { get; private set; }
    public string? Description { get; private set; }
    public MediaType Type { get; private set; }
    public DateTime UploadedAt { get; private set; }

#pragma warning disable CS8618
    private MediaItem() { }
#pragma warning restore CS8618

    public static MediaItem Create(
        string url,
        MediaType type,
        string? title = null,
        string? description = null)
    {
        return new MediaItem
        {
            Id = Guid.CreateVersion7(),
            Url = url,
            Type = type,
            Title = title,
            Description = description,
            UploadedAt = DateTime.UtcNow
        };
    }
}

public enum MediaType
{
    Image = 0,
    Video = 1,
    Document = 2
}

public class Post
{
    public Guid Id { get; private set; }
    public string Title { get; private set; } = null!;
    public string Content { get; private set; } = null!;
    public string? ThumbnailUrl { get; private set; }
    public DateTime PublishedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

#pragma warning disable CS8618
    private Post() { }
#pragma warning restore CS8618

    public static Post Create(
        string title,
        string content,
        string? thumbnailUrl = null)
    {
        return new Post
        {
            Id = Guid.CreateVersion7(),
            Title = title,
            Content = content,
            ThumbnailUrl = thumbnailUrl,
            PublishedAt = DateTime.UtcNow
        };
    }

    public void Update(string title, string content, string? thumbnailUrl = null)
    {
        Title = title;
        Content = content;
        ThumbnailUrl = thumbnailUrl;
        UpdatedAt = DateTime.UtcNow;
    }
}
