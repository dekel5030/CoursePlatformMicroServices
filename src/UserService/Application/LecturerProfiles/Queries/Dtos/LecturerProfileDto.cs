namespace Users.Application.LecturerProfiles.Queries.Dtos;

public record LecturerProfileDto(
    Guid Id,
    Guid UserId,
    string? ProfessionalBio,
    string? Expertise,
    int YearsOfExperience,
    List<ProjectDto> Projects,
    List<MediaItemDto> MediaItems,
    List<PostDto> Posts);

public record ProjectDto(
    Guid Id,
    string Title,
    string? Description,
    string? Url,
    string? ThumbnailUrl,
    DateTime CreatedAt);

public record MediaItemDto(
    Guid Id,
    string Url,
    string? Title,
    string? Description,
    string Type,
    DateTime UploadedAt);

public record PostDto(
    Guid Id,
    string Title,
    string Content,
    string? ThumbnailUrl,
    DateTime PublishedAt,
    DateTime? UpdatedAt);
