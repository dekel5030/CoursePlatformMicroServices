using Microsoft.EntityFrameworkCore;
using Users.Domain.Users;

namespace Users.Application.Abstractions.Data;

public interface IReadDbContext
{
    DbSet<User> Users { get; }
}