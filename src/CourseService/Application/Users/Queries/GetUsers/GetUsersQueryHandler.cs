using Courses.Application.Abstractions.Data;
using Courses.Application.Users.Dtos;
using Courses.Domain.Courses.Primitives;
using Courses.Domain.Users;
using Kernel;
using Kernel.Messaging.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Courses.Application.Users.Queries.GetUsers;

internal sealed class GetUsersQueryHandler : IQueryHandler<GetUsersQuery, IReadOnlyList<UserDto>>
{
    private readonly IReadDbContext _readDbContext;

    public GetUsersQueryHandler(IReadDbContext readDbContext)
    {
        _readDbContext = readDbContext;
    }

    public async Task<Result<IReadOnlyList<UserDto>>> Handle(
        GetUsersQuery request,
        CancellationToken cancellationToken = default)
    {
        IQueryable<User> query = _readDbContext.Users;

        query = ApplyFilters(request, query);

        List<UserDto> userDtos = await query
            .Select(user => UserDtoMapper.Map(user))
            .ToListAsync(cancellationToken);

        return Result.Success<IReadOnlyList<UserDto>>(userDtos);
    }

    private static IQueryable<User> ApplyFilters(GetUsersQuery request, IQueryable<User> query)
    {
        if (request.Filter.Ids is { } idsEnumerable)
        {
            var ids = idsEnumerable.Distinct().Select(id => new UserId(id)).ToList();
            if (ids.Count > 0)
            {
                query = query.Where(u => ids.Contains(u.Id));
            }
        }

        return query;
    }
}
