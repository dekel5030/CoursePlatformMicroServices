using Domain.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedKernel.Customers;
using SharedKernel.Orders;

namespace Infrastructure.Persistence.Configurations;

internal sealed class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> b)
    {
        b.ToTable("Orders");

        b.HasKey(x => x.Id);

        b.Property(x => x.Id)
            .HasConversion(v => v.Value, v => new OrderId(v));

        b.Property(x => x.CustomerId)
            .HasConversion(v => v.Value, v => new UserId(v));

        b.OwnsOne(x => x.TotalPrice, m =>
        {
            m.Property(p => p.Amount).HasColumnName("Total");
            m.Property(p => p.Currency).HasColumnName("Currency").HasMaxLength(3);
        });

        b.HasMany(o => o.Lines)
         .WithOne()
         .HasForeignKey("OrderId")
         .OnDelete(DeleteBehavior.Cascade);

        b.Navigation(o => o.Lines)
         .HasField("_items")
         .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
