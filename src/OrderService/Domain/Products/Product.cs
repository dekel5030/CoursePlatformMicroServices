using Domain.Products.Primitives;
using Kernel;
using SharedKernel;

namespace Domain.Products;

public sealed class Product : IVersionedEntity
{
    public ProductId Id { get; set; }
    public ExternalProductId ExternalId { get; set; }
    public string Name { get; set; } = null!;
    public Money Price { get; set; } = Money.Zero();
    public long EntityVersion { get; private set; }

    private Product() { }

    public static Product Create(
        ExternalProductId externalId,
        string name,
        Money price)
    {
        return new Product
        {
            Id = new ProductId(Guid.CreateVersion7()),
            ExternalId = externalId,
            Name = name,
            Price = price
        };
    }
}