using Auth.Application.AuthUsers.Queries.Dtos;
using Kernel.Messaging.Abstractions;

namespace Auth.Application.AuthUsers.Queries.GetMe;

public record GetMeQuery : IQuery<UserDto>;
