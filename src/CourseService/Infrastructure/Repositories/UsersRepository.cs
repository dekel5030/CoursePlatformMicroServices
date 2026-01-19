    using Courses.Application.Abstractions.Repositories;
    using Courses.Domain.Courses.Primitives;
    using Courses.Domain.Users;
    using Courses.Infrastructure.Database;

    namespace Courses.Infrastructure.Repositories;

    internal sealed class UsersRepository : RepositoryBase<User, UserId>, IUsersRepository
    {
        public UsersRepository(WriteDbContext dbContext) : base(dbContext)
        {
        }
    }
