using Application.Abstractions.Data;
using Application.Products.IntegrationEvents.ProductPublished;
using Domain.Products;
using Domain.Products.Primitives;
using FluentAssertions;
using Moq;
using Moq.EntityFrameworkCore;
using SharedKernel;
using Xunit;

namespace Application.UnitTests.Products.IntegrationEvents;
public class ProductPublishedIntegrationEventHandlerTests
{
    [Fact]
    public async Task Handle_ShouldInsertNewProduct_WhenNotExists()
    {
        // Arrange
        var dbContextMock = new Mock<IWriteDbContext>();
        var products = new List<Product>();
        dbContextMock.Setup(db => db.Products).ReturnsDbSet(products);

        var handler = new ProductPublishedIntegrationEventHandler(dbContextMock.Object);

        var evt = new ProductPublishedIntegrationEvent(
            ExternalId: "p-1",
            Name: "Test Product",
            Price: 100,
            Currency: "ILS",
            AggregateVersion: 1
        );

        // Act
        await handler.Handle(evt);

        // Assert
        products.Should().ContainSingle(p =>
            p.ExternalId == new ExternalProductId("p-1") &&
            p.Name == "Test Product" &&
            p.Price.Amount == 100 &&
            p.Price.Currency == "ILS"
        );
        dbContextMock.Verify(db => db.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }


    [Fact]
    public async Task Handle_ShouldUpdateProduct_WhenExists()
    {
        // Arrange
        var product = Product.Create(new ExternalProductId("p-1"), "Old Name", new Money(50, "USD"));
        var products = new List<Product> { product };

        var dbContextMock = new Mock<IWriteDbContext>();
        dbContextMock.Setup(db => db.Products).ReturnsDbSet(products);

        var handler = new ProductPublishedIntegrationEventHandler(dbContextMock.Object);

        var evt = new ProductPublishedIntegrationEvent(
            ExternalId: "p-1",
            Name: "New Name",
            Price: 200,
            Currency: "ILS",
            AggregateVersion: 2
        );

        // Act
        await handler.Handle(evt);

        // Assert
        product.Name.Should().Be("New Name");
        product.Price.Amount.Should().Be(200);
        product.Price.Currency.Should().Be("ILS");
        dbContextMock.Verify(db => db.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

}
