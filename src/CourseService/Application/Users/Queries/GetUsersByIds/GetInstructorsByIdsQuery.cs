using Courses.Application.Courses.Dtos;
using Courses.Application.Shared.Dtos;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Users.Queries.GetByIds;

public sealed record GetInstructorsByIdsQuery(
    IEnumerable<Guid> Ids) : IQuery<IReadOnlyList<UserDto>>;
