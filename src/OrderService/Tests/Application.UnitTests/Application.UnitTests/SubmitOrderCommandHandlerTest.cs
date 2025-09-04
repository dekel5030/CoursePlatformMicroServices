using Application.Orders.Commands.SubmitOrder;
using Application.Abstractions.Data;
using Domain.Orders;
using Domain.Products;
using Domain.Products.Primitives;
using Domain.Products.Errors;
using Domain.Users.Primitives;
using Domain.Users.Errors;
using Moq;
using FluentAssertions;
using Xunit;
using Domain.Users;
using SharedKernel;
using Domain.Orders.Errors;
using Moq.EntityFrameworkCore;

namespace Application.UnitTests;

public class SubmitOrderCommandHandlerTest
{
    private readonly Mock<IApplicationDbContext> _dbContextMock;
    private readonly SubmitOrderCommandHandler _handler;
    private readonly User _user1;
    private readonly Product _product1;

    public SubmitOrderCommandHandlerTest()
    {
        _dbContextMock = new Mock<IApplicationDbContext>();
        _handler = new SubmitOrderCommandHandler(_dbContextMock.Object);
        _user1 = User.Create(new ExternalUserId("user-1"), "user@email.com", "Israel Israeli", true);
        _product1 = Product.Create(new ExternalProductId("prod-1"), "Product 1", new Money(10, "ILS"));
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenUserDoesNotExist()
    {
        // Arrange
        var products = new List<SubmitOrderItemDto>();
        var dto = new SubmitOrderDto(_user1.ExternalUserId.Value, products);
        var command = new SubmitOrderCommand(dto);

        _dbContextMock.Setup(db => db.Users).ReturnsDbSet(new List<User>());

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.NotFound);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenProductNotFound()
    {
        // Arrange
        var products = new List<SubmitOrderItemDto>() { new SubmitOrderItemDto(_product1.ExternalId.Value, 1)};
        var dto = new SubmitOrderDto(_user1.ExternalUserId.Value, products);
        var command = new SubmitOrderCommand(dto);

        _dbContextMock.Setup(db => db.Users).ReturnsDbSet(new List<User> { _user1 });
        _dbContextMock.Setup(db => db.Products).ReturnsDbSet(new List<Product>());

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProductErrors.NotFound);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenLineItemCreationFails()
    {
        // Arrange
        decimal invalidQty = -1;
        var products = new List<SubmitOrderItemDto>() {
                    new SubmitOrderItemDto(_product1.ExternalId.Value, invalidQty)};
        var dto = new SubmitOrderDto(_user1.ExternalUserId.Value, products);
        var command = new SubmitOrderCommand(dto);

        _dbContextMock.Setup(db => db.Users).ReturnsDbSet(new List<User> { _user1 });
        _dbContextMock.Setup(db => db.Products).ReturnsDbSet(new List<Product> { _product1 });

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenOrderSubmitFailsOnEmptyOrder()
    {
        // Arrange
        var products = new List<SubmitOrderItemDto>();
        var dto = new SubmitOrderDto(_user1.ExternalUserId.Value, products);
        var command = new SubmitOrderCommand(dto);

        Order alreadySubmittedOrder = Order.Create(_user1.ExternalUserId);
        alreadySubmittedOrder.Submit();

        _dbContextMock.Setup(db => db.Users).ReturnsDbSet(new List<User> { _user1 });
        _dbContextMock.Setup(db => db.Products).ReturnsDbSet(new List<Product>());
        _dbContextMock.Setup(db => db.Orders).ReturnsDbSet(new List<Order> { alreadySubmittedOrder });

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OrderErrors.OrderIsEmpty);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenAllValid()
    {
        // Arrange
        var products = new List<SubmitOrderItemDto>() { new SubmitOrderItemDto(_product1.ExternalId.Value, 1) };
        var dto = new SubmitOrderDto(_user1.ExternalUserId.Value, products);
        var command = new SubmitOrderCommand(dto);

        _dbContextMock.Setup(db => db.Users).ReturnsDbSet(new List<User> { _user1 });
        _dbContextMock.Setup(db => db.Products).ReturnsDbSet(new List<Product> { _product1 });
        _dbContextMock.Setup(db => db.Orders).ReturnsDbSet(new List<Order>());
        _dbContextMock.Setup(db => db.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
    }
}
