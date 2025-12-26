using Auth.Application.AuthUsers.Queries.Dtos;
using Kernel.Messaging.Abstractions;

namespace Auth.Application.AuthUsers.Queries.GetUserById;

public record GetUserByIdQuery(Guid UserId) : IQuery<UserDto>;
