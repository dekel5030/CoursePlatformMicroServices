using Kernel.Messaging.Abstractions;

namespace Auth.Application.AuthUsers.Queries;

public record GetMeQuery : IQuery<UserDto>;
