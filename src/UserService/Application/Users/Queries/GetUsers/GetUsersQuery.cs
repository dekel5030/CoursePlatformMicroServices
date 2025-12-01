using Application.Abstractions.Messaging;
using Application.Users.Queries.Dtos;

namespace Application.Users.Queries.GetUsers;

public sealed record GetUsersQuery(int PageNumber = 1, int PageSize = 10) : IQuery<CollectionDto<UserReadDto>> { }