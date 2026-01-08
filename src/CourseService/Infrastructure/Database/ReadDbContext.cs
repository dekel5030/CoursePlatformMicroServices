using Courses.Application.Abstractions.Data;
using Microsoft.EntityFrameworkCore;

namespace Courses.Infrastructure.Database;

public class ReadDbContext : AppDbContextBase, IReadDbContext
{
    public ReadDbContext(DbContextOptions<ReadDbContext> options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
    }
}
