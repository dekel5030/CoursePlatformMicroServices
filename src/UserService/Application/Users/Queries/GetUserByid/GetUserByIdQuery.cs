using Application.Abstractions.Messaging;
using Application.Users.Queries.Dtos;

namespace Application.Users.Queries.GetUserByid;

public sealed record GetUserByIdQuery(Guid UserId) : IQuery<UserReadDto>;