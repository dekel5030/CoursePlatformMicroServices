using Domain.Permissions;
using Domain.Roles;
using Kernel.Auth.AuthTypes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal sealed class RoleConfiguration : IEntityTypeConfiguration<Role> 
{ 
    public void Configure(EntityTypeBuilder<Role> builder) 
    { 
        builder.ToTable("domain_roles");

        builder.Property(r => r.Id).ValueGeneratedNever();

        builder.HasMany(r => r.Permissions)
            .WithOne()
            .HasForeignKey("RoleId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(r => r.DomainEvents);
    } 
}