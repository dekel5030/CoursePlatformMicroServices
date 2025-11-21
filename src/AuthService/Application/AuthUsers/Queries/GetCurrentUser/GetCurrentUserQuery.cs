using Application.Abstractions.Messaging;
using Application.AuthUsers.Dtos;

namespace Application.AuthUsers.Queries.GetCurrentUser;

public sealed record GetCurrentUserQuery(string Email) : IQuery<AuthResponseDto>;
