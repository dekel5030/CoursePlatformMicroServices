using Domain.Users;
using Kernel;
using Kernel.Messaging.Abstractions;
using Microsoft.EntityFrameworkCore;
using Users.Application.Abstractions.Data;
using Users.Application.Users.Queries.Dtos;

namespace Users.Application.Users.Queries.GetUsers;

public class GetUsersQueryHandler(IReadDbContext DbContext) : IQueryHandler<GetUsersQuery, CollectionDto<UserReadDto>>
{
    public async Task<Result<CollectionDto<UserReadDto>>> Handle(
        GetUsersQuery request,
        CancellationToken cancellationToken = default)
    {
        if (request.PageNumber <= 0 || request.PageSize <= 0)
        {
            return Result.Failure<CollectionDto<UserReadDto>>(
                Error.Validation("Users.InvalidPagination", "PageNumber and PageSize must be greater than 0."));
        }

        IQueryable<User> query = DbContext.Users.AsNoTracking();

        var totalCount = await query.CountAsync(cancellationToken);

        List<UserReadDto> items = await query
            .OrderBy(u => u.Id)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(u => new UserReadDto(
                u.Id.Value,
                u.Email,
                u.FullName != null ? u.FullName.FirstName : null,
                u.FullName != null ? u.FullName.LastName : null,
                u.DateOfBirth,
                u.PhoneNumber != null ? u.PhoneNumber.ToString() : null))
            .ToListAsync(cancellationToken);

        var dto = new CollectionDto<UserReadDto>
        {
            Items = items,
            TotalCount = totalCount,
            Page = request.PageNumber,
            PageSize = request.PageSize
        };

        return Result.Success(dto);
    }
}
