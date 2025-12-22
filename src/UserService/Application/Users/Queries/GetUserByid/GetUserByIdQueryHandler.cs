using Domain.Users.Errors;
using Domain.Users.Primitives;
using Kernel;
using Kernel.Messaging.Abstractions;
using Microsoft.EntityFrameworkCore;
using Users.Application.Abstractions.Data;
using Users.Application.Users.Queries.Dtos;

namespace Users.Application.Users.Queries.GetUserByid;

public class GetUserByIdQueryHandler(IReadDbContext DbContext) : IQueryHandler<GetUserByIdQuery, UserReadDto>
{
    public async Task<Result<UserReadDto>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken = default)
    {
        UserReadDto? user = await DbContext.Users
            .Where(u => u.Id == new UserId(request.UserId))
            .Select(u => new UserReadDto(
                u.Id.Value,
                u.Email,
                u.FullName == null ? null : u.FullName.FirstName,
                u.FullName == null ? null : u.FullName.LastName,
                u.DateOfBirth,
                u.PhoneNumber == null ? null : u.PhoneNumber.ToString()
            ))
            .FirstOrDefaultAsync(cancellationToken);

        if (user is null)
        {
            return Result.Failure<UserReadDto>(UserErrors.NotFound);
        }

        return Result.Success(user);
    }
}