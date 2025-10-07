using Application.Abstractions.Messaging;

namespace Application.Users.Queries.GetUserByid;

public sealed record GetUserByIdQuery(Guid UserId) : IQuery<UserReadDto>;