using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Users.Errors;
using Kernel;
using Microsoft.EntityFrameworkCore;

namespace Application.Users.Queries.GetUserByid;

public class GetUserByIdQueryHandler(IReadDbContext DbContext) : IQueryHandler<GetUserByIdQuery, UserReadDto>
{
    public async Task<Result<UserReadDto>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken = default)
    {
        UserReadDto? user = await DbContext.Users
            .Where(u => u.Id.Value == request.UserId)
            .Select(u => new UserReadDto(
                u.Id.Value,
                u.Email,
                u.FullName?.FirstName,
                u.FullName?.LastName,
                u.DateOfBirth,
                u.PhoneNumber != null ? u.PhoneNumber.ToString() : null))
            .FirstOrDefaultAsync(cancellationToken);

        if (user is null)
        {
            return Result.Failure<UserReadDto>(UserErrors.NotFound);
        }

        return Result.Success(user);
    }
}
