using Auth.Application.AuthUsers.Queries.Dtos;
using Kernel.Messaging.Abstractions;

namespace Auth.Application.AuthUsers.Queries.GetAllUsers;

public sealed record GetAllUsersQuery : IQuery<IReadOnlyList<UserDto>>;
