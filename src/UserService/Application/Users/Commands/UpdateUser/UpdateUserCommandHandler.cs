using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Users;
using Domain.Users.Errors;
using Domain.Users.Primitives;
using Kernel;
using Microsoft.EntityFrameworkCore;

namespace Application.Users.Commands.UpdateUser;

public class UpdateUserCommandHandler(IWriteDbContext dbContext) 
    : ICommandHandler<UpdateUserCommand, UpdatedUserResponseDto>
{
    public async Task<Result<UpdatedUserResponseDto>> Handle(
        UpdateUserCommand request,
        CancellationToken cancellationToken = default)
    {
        var userId = new UserId(request.UserId);
        User? user = await dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user is null)
        {
            return Result.Failure<UpdatedUserResponseDto>(UserErrors.NotFound);
        }

        FullName? fullName = null;
        if (request.FirstName is not null && request.LastName is not null)
        {
            fullName = new FullName(request.FirstName, request.LastName);
        }

        Result updateResult = user.UpdateProfile(
            fullName: fullName,
            phoneNumber: request.PhoneNumber,
            dateOfBirth: request.DateOfBirth);

        if (updateResult.IsFailure)
        {
            return Result.Failure<UpdatedUserResponseDto>(updateResult.Error!);
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        var response = new UpdatedUserResponseDto(
            user.Id.Value,
            user.Email,
            user.FullName?.FirstName,
            user.FullName?.LastName,
            user.DateOfBirth,
            user.PhoneNumber?.ToString());

        return Result.Success(response);
    }
}
