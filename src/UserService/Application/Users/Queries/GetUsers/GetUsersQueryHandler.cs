using Kernel;
using Kernel.Messaging.Abstractions;
using Microsoft.EntityFrameworkCore;
using Users.Application.Abstractions.Data;
using Users.Application.Users.Queries.Dtos;
using Users.Domain.Users;

namespace Users.Application.Users.Queries.GetUsers;

public class GetUsersQueryHandler(IReadDbContext dbContext) : IQueryHandler<GetUsersQuery, CollectionDto<UserReadDto>>
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

        IQueryable<User> query = dbContext.Users.AsNoTracking();

        int totalCount = await query.CountAsync(cancellationToken);

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
                u.PhoneNumber != null ? u.PhoneNumber.ToString() : null,
                u.AvatarUrl,
                u.Bio,
                u.LinkedInUrl,
                u.GitHubUrl,
                u.TwitterUrl,
                u.WebsiteUrl,
                u.IsLecturer))
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
