using Application.Users.Queries.Dtos;
using Kernel.Messaging.Abstractions;

namespace Application.Users.Queries.GetUserByid;

public sealed record GetUserByIdQuery(Guid UserId) : IQuery<UserReadDto>;