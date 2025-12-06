using Application.Abstractions.Messaging;
using Application.AuthUsers.Dtos;

namespace Application.AuthUsers.Queries.GetCurrentUser;

public sealed record GetCurrentUserQuery(Guid UserId) : IQuery<CurrentUserDto>;
