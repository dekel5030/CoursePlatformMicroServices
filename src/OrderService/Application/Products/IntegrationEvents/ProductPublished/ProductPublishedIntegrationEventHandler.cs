using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Microsoft.EntityFrameworkCore;
using Domain.Products;
using SharedKernel;
using Domain.Products.Primitives;
using Kernel;

namespace Application.Products.IntegrationEvents.ProductPublished;

public class ProductPublishedIntegrationEventHandler : IIntegrationEventHandler<ProductPublishedIntegrationEvent>
{
    private readonly IWriteDbContext _dbContext;

    public ProductPublishedIntegrationEventHandler(IWriteDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Handle(
        ProductPublishedIntegrationEvent request, 
        CancellationToken cancellationToken = default)
    {
        ExternalProductId externalProductId = new ExternalProductId(request.ExternalId);

        Product? product = await _dbContext.Products
            .SingleOrDefaultAsync(p => p.ExternalId == externalProductId, cancellationToken);

        if (product is null)
        {
            Money price = new Money(request.Price, request.Currency);

            product = Product.Create(
                externalId: externalProductId,
                name: request.Name,
                price: price
            );

            await _dbContext.Products.AddAsync(product, cancellationToken);
        }
        else // if (product.Version < request.AggregateVersion)
        {
            product.Name = request.Name;
            product.Price = new Money(request.Price, request.Currency);

        }
        //else
        //{
        //    return;
        //}

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}