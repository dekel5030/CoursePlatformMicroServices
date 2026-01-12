using Domain.Users;
using Domain.Users.Errors;
using Domain.Users.Primitives;
using Kernel;
using Kernel.Messaging.Abstractions;
using Microsoft.EntityFrameworkCore;
using Users.Application.Abstractions.Data;

namespace Users.Application.Users.Commands.CreateUser;

internal sealed class CreateUserCommandHandler(IWriteDbContext dbContext)
    : ICommandHandler<CreateUserCommand, CreatedUserRespondDto>
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

        // Note: Manual user creation is deprecated. Users should be created via AuthService events.
        // This creates a temporary AuthUserId for backward compatibility.

        // Parse userId if provided, otherwise it will be generated
        UserId? userId = null;
        if (!string.IsNullOrEmpty(request.UserId) && Guid.TryParse(request.UserId, out Guid userIdGuid))
        {
            userId = new UserId(userIdGuid);
        }

        Result<User> creationResult = User.CreateUser(
            id: userId ?? new UserId(Guid.CreateVersion7()),
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
