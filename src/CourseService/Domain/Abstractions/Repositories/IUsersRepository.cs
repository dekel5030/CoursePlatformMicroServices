using Courses.Domain.Courses.Primitives;
using Courses.Domain.Users;

namespace Courses.Domain.Abstractions.Repositories;

public interface IUsersRepository : IRepository<User, UserId>;
