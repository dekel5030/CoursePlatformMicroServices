using Courses.Application.Abstractions.Repositories;
using Courses.Domain.Courses.Primitives;
using Courses.Domain.Users;
using Courses.Infrastructure.Database.Write;

namespace Courses.Infrastructure.Database.Repositories;

internal sealed class UsersRepository : RepositoryBase<User, UserId>, IUsersRepository
{
    public UsersRepository(WriteDbContext dbContext) : base(dbContext)
    {
    }
}
