using Domain.Permissions;
using Kernel.Auth.AuthTypes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.DomainConfigurations;

internal sealed class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable("domain_permissions");

        builder.Property(p => p.Id).ValueGeneratedNever();

        builder.Property(p => p.Effect)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(p => p.Action)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(p => p.Resource)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(p => p.ResourceId)
            .HasConversion(
                id => id.Value, 
                val => ResourceId.Create(val))
            .HasMaxLength(255)
            .IsRequired();

        // Ignore domain events from Entity base class
        builder.Ignore(p => p.DomainEvents);

        // Add index for common queries
        builder.HasIndex(p => new { p.Effect, p.Action, p.Resource, p.ResourceId });
    }
}
