using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Users;
using Domain.Users.Errors;
using Domain.Users.Primitives;
using Kernel;
using Microsoft.EntityFrameworkCore;

namespace Application.Users.Commands.CreateUser;

public class CreateUserCommandHandler(IWriteDbContext dbContext) : ICommandHandler<CreateUserCommand, CreatedUserRespondDto>
{
    public async Task<Result<CreatedUserRespondDto>> Handle(
        CreateUserCommand request,
        CancellationToken cancellationToken = default)
    {
        if (await dbContext.Users.AnyAsync(u => u.Email == request.Email, cancellationToken))
        {
            return Result.Failure<CreatedUserRespondDto>(UserErrors.EmailAlreadyExists);
        }

        FullName? userFullName = null;
        if (request.FirstName is not null && request.LastName is not null)
        {
            userFullName = new(request.FirstName, request.LastName);
        }

        Result<User> creationResult = User.CreateUser(
            email: request.Email,
            fullName: userFullName,
            phoneNumber: request.PhoneNumber,
            dateOfBirth: request.DateOfBirth);

        if (creationResult.IsFailure)
        {
            return Result.Failure<CreatedUserRespondDto>(creationResult.Error!);
        }

        User createdUser = creationResult.Value;
        await dbContext.Users.AddAsync(createdUser, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        CreatedUserRespondDto response = new(createdUser.Id.Value);
        return Result.Success(response);
    }
}
