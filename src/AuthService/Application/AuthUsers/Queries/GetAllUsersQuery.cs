using Kernel.Messaging.Abstractions;

namespace Auth.Application.AuthUsers.Queries;

public sealed record GetAllUsersQuery : IQuery<IReadOnlyList<UserDto>>;
