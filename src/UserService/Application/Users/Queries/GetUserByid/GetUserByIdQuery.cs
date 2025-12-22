using Kernel.Messaging.Abstractions;
using Users.Application.Users.Queries.Dtos;

namespace Users.Application.Users.Queries.GetUserByid;

public sealed record GetUserByIdQuery(Guid UserId) : IQuery<UserReadDto>;