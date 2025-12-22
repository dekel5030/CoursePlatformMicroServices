using Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Users.Application.Abstractions.Data;

public interface IReadDbContext
{
    DbSet<User> Users { get; }
}