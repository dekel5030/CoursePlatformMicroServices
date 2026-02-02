using Courses.Application.Abstractions.Data;
using Courses.Application.Courses.Dtos;
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

        if (request.Filter.CourseId is { } courseId)
        {
            List<UserId> instructorIds = await _readDbContext.Courses
                .Where(c => c.Id == courseId)
                .Select(c => c.InstructorId)
                .ToListAsync(cancellationToken);

            if (instructorIds.Count == 0)
            {
                return Result.Success<IReadOnlyList<UserDto>>([]);
            }

            query = query.Where(u => instructorIds.Contains(u.Id));
        }

        if (request.Filter.Ids is { } idsEnumerable)
        {
            var ids = idsEnumerable.Distinct().Select(id => new UserId(id)).ToList();
            if (ids.Count > 0)
            {
                query = query.Where(u => ids.Contains(u.Id));
            }
        }

        List<UserDto> userDtos = await query
            .Select(u => new UserDto
            {
                Id = u.Id.Value,
                FirstName = u.FirstName,
                LastName = u.LastName,
                AvatarUrl = u.AvatarUrl
            })
            .ToListAsync(cancellationToken);

        return Result.Success<IReadOnlyList<UserDto>>(userDtos);
    }
}
