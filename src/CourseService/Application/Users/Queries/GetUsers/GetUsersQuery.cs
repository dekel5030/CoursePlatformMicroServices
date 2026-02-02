using Courses.Application.Courses.Dtos;
using Courses.Domain.Courses.Primitives;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Users.Queries.GetUsers;

public sealed record UserFilter(
    CourseId? CourseId = null,
    IEnumerable<Guid>? Ids = null);

public sealed record GetUsersQuery(UserFilter Filter) : IQuery<IReadOnlyList<UserDto>>;
