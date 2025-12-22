using Application.Users.Queries.Dtos;
using Kernel.Messaging.Abstractions;

namespace Application.Users.Queries.GetUsers;

public sealed record GetUsersQuery(int PageNumber = 1, int PageSize = 10) : IQuery<CollectionDto<UserReadDto>> { }