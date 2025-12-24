using Kernel.Messaging.Abstractions;

namespace Auth.Application.AuthUsers.Queries;

public record GetUserByIdQuery(Guid UserId) : IQuery<UserDto>;
