using Microsoft.EntityFrameworkCore;
using SharedKernel.VersionedEntity;

namespace Infrastructure.VersionedEntity;

internal static class VersionedEntityModelBuilderExtensions
{
    public const string VersionFieldName = "entity_version";

    internal static void AddShadowVersion(this ModelBuilder modelBuilder)
    {
        foreach (var entity in modelBuilder.Model.GetEntityTypes().ToList())
        {
            if (entity.IsOwned() || entity.GetTableName() is null) continue;
            if (!typeof(IVersionedEntity).IsAssignableFrom(entity.ClrType)) continue;

            modelBuilder.Entity(entity.ClrType)
                        .Property<long>(VersionFieldName)
                        .IsConcurrencyToken();
        }
    }
}
