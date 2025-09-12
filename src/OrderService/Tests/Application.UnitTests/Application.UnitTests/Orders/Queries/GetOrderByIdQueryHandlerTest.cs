using Application.Abstractions.Data;
using Application.Orders.Queries.GetById;
using Domain.Orders;
using Domain.Orders.Errors;
using Domain.Orders.Primitives;
using Domain.Users.Primitives;
using Domain.Products.Primitives;
using FluentAssertions;
using Moq;
using Moq.EntityFrameworkCore;
using SharedKernel;
using Xunit;

namespace Application.UnitTests.Orders.Queries;

public class GetOrderByIdQueryHandlerTest
{
    private readonly Mock<IWriteDbContext> _dbContextMock;
    private readonly GetOrderByIdQueryHandler _handler;
    private readonly Order _order;

    public GetOrderByIdQueryHandlerTest()
    {
        _dbContextMock = new Mock<IWriteDbContext>();
        _handler = new GetOrderByIdQueryHandler(_dbContextMock.Object);

        var externalUserId = new ExternalUserId("user-1");
        _order = Order.Create(externalUserId);

        var lineItem = LineItem.Create(
            new ExternalProductId("prod-1"),
            2,
            "Test Product",
            new Money(10, "ILS")
        ).Value;

        _order.AddLine(lineItem);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenOrderExists()
    {
        // Arrange
        var query = new GetOrderByIdQuery(_order.Id);
        var orders = new List<Order> { _order };

        _dbContextMock.Setup(db => db.Orders).ReturnsDbSet(orders);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.OrderId.Should().Be(_order.Id.Value);
        result.Value.Total.Should().Be(_order.TotalPrice);
        result.Value.Lines.Should().HaveCount(_order.Lines.Count);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenOrderNotFound()
    {
        // Arrange
        var nonExistentOrderId = new OrderId(Guid.NewGuid());
        var query = new GetOrderByIdQuery(nonExistentOrderId);
        var orders = new List<Order>();

        _dbContextMock.Setup(db => db.Orders).ReturnsDbSet(orders);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OrderErrors.NotFound);
    }

    [Fact]
    public async Task Handle_ShouldReturnOrderWithCorrectLineItems_WhenOrderHasMultipleLineItems()
    {
        // Arrange
        var query = new GetOrderByIdQuery(_order.Id);

        var secondLineItem = LineItem.Create(
            new ExternalProductId("prod-2"),
            1,
            "Second Product",
            new Money(20, "ILS")
        ).Value;
        _order.AddLine(secondLineItem);

        var orders = new List<Order> { _order };
        _dbContextMock.Setup(db => db.Orders).ReturnsDbSet(orders);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Lines.Should().HaveCount(2);
        result.Value.Lines.Should().Contain(li => li.ExternalProductId == "prod-1");
        result.Value.Lines.Should().Contain(li => li.ExternalProductId == "prod-2");
        result.Value.Total.Amount.Should().Be(40); // 2*10 + 1*20
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyLineItems_WhenOrderHasNoLineItems()
    {
        // Arrange
        var emptyOrder = Order.Create(new ExternalUserId("user-2"));
        var query = new GetOrderByIdQuery(emptyOrder.Id);
        var orders = new List<Order> { emptyOrder };

        _dbContextMock.Setup(db => db.Orders).ReturnsDbSet(orders);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Lines.Should().BeEmpty();
        result.Value.Total.Amount.Should().Be(0);
    }

    [Fact]
    public async Task Handle_ShouldReturnCorrectLineItemData_WhenOrderExists()
    {
        // Arrange
        var query = new GetOrderByIdQuery(_order.Id);
        var orders = new List<Order> { _order };

        _dbContextMock.Setup(db => db.Orders).ReturnsDbSet(orders);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        var lineItem = result.Value.Lines.First();
        lineItem.ExternalProductId.Should().Be("prod-1");
        lineItem.Quantity.Should().Be(2);
        lineItem.Name.Should().Be("Test Product");
        lineItem.UnitPrice.Amount.Should().Be(10);
        lineItem.LineTotal.Amount.Should().Be(20);
    }
}

