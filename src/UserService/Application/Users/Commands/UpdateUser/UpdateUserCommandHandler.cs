using Domain.Users;
using Domain.Users.Errors;
using Domain.Users.Primitives;
using Kernel;
using Kernel.Auth.AuthTypes;
using Kernel.Messaging.Abstractions;
using Users.Application.Abstractions.Context;
using Users.Application.Abstractions.Data;

namespace Users.Application.Users.Commands.UpdateUser;

public class UpdateUserCommandHandler(IWriteDbContext dbContext, ICurrentUserContext currentUser)
    : ICommandHandler<UpdateUserCommand, UpdatedUserResponseDto>
{
    public async Task<Result<UpdatedUserResponseDto>> Handle(
        UpdateUserCommand request,
        CancellationToken cancellationToken = default)
    {
        var userId = new UserId(request.UserId);
        var user = await dbContext.Users.FindAsync([userId], cancellationToken);

        if (user is null)
        {
            return Result.Failure<UpdatedUserResponseDto>(UserErrors.NotFound);
        }

        if (currentUser.UserId != user.Id.Value &&
            !currentUser.HasPermissionOnUsersResource(ActionType.Update, user.Id.Value))
        {
            return Result.Failure<UpdatedUserResponseDto>(UserErrors.Forbidden);
        }

        FullName currentFullName = user.FullName ?? new FullName(string.Empty, string.Empty);

        string firstName = request.FirstName ?? currentFullName.FirstName;
        string lastName = request.LastName ?? currentFullName.LastName;

        var updatedFullName = new FullName(firstName, lastName);

        Result updateResult = user.UpdateProfile(
            fullName: updatedFullName,
            phoneNumber: request.PhoneNumber,
            dateOfBirth: request.DateOfBirth);

        if (updateResult.IsFailure)
        {
            return Result.Failure<UpdatedUserResponseDto>(updateResult.Error!);
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(MapToDto(user));
    }

    private static UpdatedUserResponseDto MapToDto(User user)
    {
        return new UpdatedUserResponseDto(
            user.Id.Value,
            user.Email,
            user.FullName?.FirstName,
            user.FullName?.LastName,
            user.DateOfBirth,
            user.PhoneNumber?.ToString());
    }
}