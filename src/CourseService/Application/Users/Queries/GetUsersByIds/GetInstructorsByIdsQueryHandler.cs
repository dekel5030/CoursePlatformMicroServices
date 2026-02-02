using Courses.Application.Abstractions.Data;
using Courses.Application.Courses.Dtos;
using Courses.Domain.Courses.Primitives;
using Courses.Domain.Users;
using Kernel;
using Kernel.Messaging.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Courses.Application.Users.Queries.GetByIds;

internal sealed class GetInstructorsByIdsQueryHandler
    : IQueryHandler<GetInstructorsByIdsQuery, IReadOnlyList<UserDto>>
{
    private readonly IReadDbContext _readDbContext;

    public GetInstructorsByIdsQueryHandler(IReadDbContext readDbContext)
    {
        _readDbContext = readDbContext;
    }

    public async Task<Result<IReadOnlyList<UserDto>>> Handle(
        GetInstructorsByIdsQuery request,
        CancellationToken cancellationToken = default)
    {
        var ids = request.Ids
            .Distinct()
            .Select(id => new UserId(id))
            .ToList();
        if (ids.Count == 0)
        {
            return Result.Success<IReadOnlyList<UserDto>>([]);
        }

        List<User> instructors = await _readDbContext.Users
            .Where(u => ids.Contains(u.Id))
            .ToListAsync(cancellationToken);

        var response = instructors.Select(instructor => new UserDto
        {
            Id = instructor.Id.Value,
            FirstName = instructor.FirstName,
            LastName = instructor.LastName,
            AvatarUrl = instructor.AvatarUrl
        })
        .ToList();

        return Result.Success<IReadOnlyList<UserDto>>(response);
    }
}
