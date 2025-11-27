using Application.Abstractions.Messaging;
using Application.Admin.Dtos;

namespace Application.Admin.Queries.GetAllUsers;

public record GetAllUsersQuery : IQuery<IEnumerable<UserDto>>;
