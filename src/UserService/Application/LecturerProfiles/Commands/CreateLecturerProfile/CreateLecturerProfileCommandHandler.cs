using Kernel;
using Kernel.Messaging.Abstractions;
using Users.Application.Abstractions.Context;
using Users.Application.Abstractions.Data;
using Users.Domain.Users;
using Users.Domain.Users.Errors;
using Users.Domain.Users.Primitives;

namespace Users.Application.LecturerProfiles.Commands.CreateLecturerProfile;

public class CreateLecturerProfileCommandHandler(
    IWriteDbContext dbContext,
    ICurrentUserContext currentUser)
    : ICommandHandler<CreateLecturerProfileCommand, LecturerProfileResponseDto>
{
    public async Task<Result<LecturerProfileResponseDto>> Handle(
        CreateLecturerProfileCommand request,
        CancellationToken cancellationToken = default)
    {
        var userId = new UserId(request.UserId);
        User? user = await dbContext.Users.FindAsync([userId], cancellationToken);

        if (user is null)
        {
            return Result.Failure<LecturerProfileResponseDto>(UserErrors.NotFound);
        }

        if (currentUser.UserId != user.Id.Value)
        {
            return Result.Failure<LecturerProfileResponseDto>(UserErrors.Forbidden);
        }

        Result createResult = user.CreateLecturerProfile(
            request.ProfessionalBio,
            request.Expertise,
            request.YearsOfExperience);

        if (createResult.IsFailure)
        {
            return Result.Failure<LecturerProfileResponseDto>(createResult.Error!);
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(new LecturerProfileResponseDto(
            user.LecturerProfile!.Id.Value,
            user.Id.Value,
            user.LecturerProfile.ProfessionalBio,
            user.LecturerProfile.Expertise,
            user.LecturerProfile.YearsOfExperience));
    }
}
