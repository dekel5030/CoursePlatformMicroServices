using Application.Abstractions.Messaging;
using Auth.Contracts.Dtos;

namespace Application.AuthUsers.Queries.GetUserAuthData;

public sealed record GetUserAuthDataQuery : IQuery<UserAuthDataDto>;
