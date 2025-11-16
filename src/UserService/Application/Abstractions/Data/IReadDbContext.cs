using Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Application.Abstractions.Data;

public interface IReadDbContext
{
    DbSet<User> Users { get; }
}
