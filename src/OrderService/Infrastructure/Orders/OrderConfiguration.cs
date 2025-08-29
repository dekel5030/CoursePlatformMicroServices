using Domain.Orders;
using Domain.Orders.Primitives;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Orders;

internal sealed class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasConversion(v => v.Value, v => new OrderId(v));

        builder.Property(x => x.CustomerId)
            .HasConversion(v => v.Value, v => new UserId(v));

        builder.OwnsOne(x => x.TotalPrice, m =>
        {
            m.Property(p => p.Amount).HasColumnName("Total");
            m.Property(p => p.Currency).HasColumnName("Currency").HasMaxLength(3);
        });

        builder.HasMany(o => o.Lines)
         .WithOne()
         .HasForeignKey("OrderId")
         .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(o => o.Lines)
         .HasField("_items")
         .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
