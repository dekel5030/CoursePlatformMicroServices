using Courses.Domain.Courses.Primitives;
using Courses.Domain.Users;

namespace Courses.Application.Abstractions.Repositories;

public interface IUsersRepository : IRepository<User, UserId>
{
    Task AddAsync(User entity, CancellationToken cancellationToken = default);
}
