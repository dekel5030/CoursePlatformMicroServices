using Domain.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedKernel.Orders;
using SharedKernel.Products;

namespace Infrastructure.Orders;

internal sealed class LineItemConfiguration : IEntityTypeConfiguration<LineItem>
{
    public void Configure(EntityTypeBuilder<LineItem> b)
    {
        b.ToTable("OrderLines");

        b.HasKey(x => x.Id);

        b.Property(x => x.Id)
            .HasConversion(v => v.Value, v => new LineItemId(v))
            .ValueGeneratedNever();

        b.Property(x => x.ProductId)
            .HasConversion(v => v.Value, v => new ProductId(v));

        b.Property<OrderId>("OrderId")
            .HasConversion(v => v.Value, v => new OrderId(v));

        b.HasOne<Order>()
         .WithMany(o => o.Lines) 
         .HasForeignKey("OrderId")
         .OnDelete(DeleteBehavior.Cascade);

        b.Property(x => x.Quantity).HasPrecision(18, 2).IsRequired();
        b.Property(x => x.Name).HasMaxLength(256).IsRequired();


        b.OwnsOne(x => x.UnitPrice, m =>
        {
            m.Property(p => p.Amount).HasColumnName("UnitPrice").HasPrecision(18, 2);
            m.Property(p => p.Currency).HasColumnName("Currency").HasMaxLength(3).IsRequired();
        });
    }
}
