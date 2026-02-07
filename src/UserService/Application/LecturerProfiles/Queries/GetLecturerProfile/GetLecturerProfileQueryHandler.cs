using Kernel;
using Kernel.Messaging.Abstractions;
using Microsoft.EntityFrameworkCore;
using Users.Application.Abstractions.Data;
using Users.Application.LecturerProfiles.Queries.Dtos;
using Users.Domain.Users;
using Users.Domain.Users.Primitives;

namespace Users.Application.LecturerProfiles.Queries.GetLecturerProfile;

public class GetLecturerProfileQueryHandler(IReadDbContext dbContext)
    : IQueryHandler<GetLecturerProfileQuery, LecturerProfileDto>
{
    public async Task<Result<LecturerProfileDto>> Handle(
        GetLecturerProfileQuery request,
        CancellationToken cancellationToken = default)
    {
        var userId = new UserId(request.UserId);

        LecturerProfile? profile = await dbContext.LecturerProfiles
            .AsNoTracking()
            .Where(lp => lp.UserId == userId)
            .FirstOrDefaultAsync(cancellationToken);

        if (profile is null)
        {
            return Result.Failure<LecturerProfileDto>(
                Error.NotFound("LecturerProfile.NotFound", "Lecturer profile not found"));
        }

        var dto = new LecturerProfileDto(
            profile.Id.Value,
            profile.UserId.Value,
            profile.ProfessionalBio,
            profile.Expertise,
            profile.YearsOfExperience,
            profile.Projects.Select(p => new ProjectDto(
                p.Id,
                p.Title,
                p.Description,
                p.Url,
                p.ThumbnailUrl,
                p.CreatedAt)).ToList(),
            profile.MediaItems.Select(m => new MediaItemDto(
                m.Id,
                m.Url,
                m.Title,
                m.Description,
                m.Type.ToString(),
                m.UploadedAt)).ToList(),
            profile.Posts.Select(p => new PostDto(
                p.Id,
                p.Title,
                p.Content,
                p.ThumbnailUrl,
                p.PublishedAt,
                p.UpdatedAt)).ToList());

        return Result.Success(dto);
    }
}
