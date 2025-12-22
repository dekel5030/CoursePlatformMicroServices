using Kernel.Messaging.Abstractions;
using Users.Application.Users.Queries.Dtos;

namespace Users.Application.Users.Queries.GetUsers;

public sealed record GetUsersQuery(int PageNumber = 1, int PageSize = 10) : IQuery<CollectionDto<UserReadDto>> { }