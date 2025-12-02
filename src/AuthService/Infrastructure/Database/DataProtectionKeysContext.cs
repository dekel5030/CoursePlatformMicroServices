using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database;

public class DataProtectionKeysContext : DbContext, IDataProtectionKeyContext
{
    public DataProtectionKeysContext(DbContextOptions<DataProtectionKeysContext> options) : base(options)
    {
        
    }

    public DbSet<DataProtectionKey> DataProtectionKeys { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema(Schemas.Keys);
    }
}
