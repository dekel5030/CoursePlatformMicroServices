using Common.Messaging.EventEnvelope;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Infrastructure.VersionedEntity;

internal sealed class VersionedEntityInterceptor : SaveChangesInterceptor
{
    public const string VersionFieldName = "entity_version";

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData, 
        InterceptionResult<int> result, 
        CancellationToken cancellationToken = default)
    {
        DbContext context = eventData.Context!;


        foreach (var versionedEntity in context.ChangeTracker.Entries<IVersionedEntity>())
        {
            var property = versionedEntity.Property(VersionFieldName);

            switch (versionedEntity.State)
            {
                case EntityState.Added:
                    if (property.CurrentValue is null)
                        property.CurrentValue = 1L;
                    break;

                case EntityState.Modified:
                    var original = property.OriginalValue as long? ?? 0L;
                    property.CurrentValue = original + 1L;
                    break;
            }
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}