using System;
using System.Collections.Generic;
using System.Text;
using Courses.Domain.Courses;
using Courses.Domain.Courses.Primitives;
using Courses.Domain.Users;

namespace Courses.Application.Abstractions.Repositories;

public interface IUsersRepository : IRepository<User, UserId>
{
    Task AddAsync(User entity, CancellationToken cancellationToken = default);
}
