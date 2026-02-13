using Courses.Application.Users.Dtos;
using Courses.Domain.Courses.Primitives;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Users.Queries.GetUsers;

public sealed record GetUsersQuery(UserFilter Filter) : IQuery<IReadOnlyList<UserDto>>;

public sealed record UserFilter(IEnumerable<Guid>? Ids = null);
