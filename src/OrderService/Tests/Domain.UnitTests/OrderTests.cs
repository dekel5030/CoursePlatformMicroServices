using Domain.Orders;
using Domain.Orders.Errors;
using Domain.Orders.Primitives;
using Domain.Users.Primitives;
using FluentAssertions;
using SharedKernel;
using Xunit;

namespace Domain.UnitTests;

public class OrderTests
{
    private readonly ExternalUserId _externalUserId = new("user-1");
    private readonly ExternalUserId _otherUserId = new("user-2");

    private readonly LineItem _lineItem1 = LineItem.Create(
            new Domain.Products.Primitives.ExternalProductId("prod-1"),
            1,
            "Product prod-1",
            new Money(10, "ILS")).Value;

    private readonly LineItem _lineItem2 = LineItem.Create(
            new Domain.Products.Primitives.ExternalProductId("prod-2"),
            2,
            "Product prod-2",
            new Money(15, "ILS")
        ).Value;

    [Fact]
    public void Create_ShouldInitializeOrder_WhenExternalUserIdProvided()
    {
        // Arrange

        // Act
        var order = Order.Create(_externalUserId);

        // Assert
        order.Should().NotBeNull();
        order.Status.Should().Be(OrderStatus.Draft);
        order.TotalPrice.Amount.Should().Be(0);
        order.Lines.Should().BeEmpty();
    }

    [Fact]
    public void AddLine_ShouldAddLineItemAndRecalculateTotal_WhenLineItemIsValid()
    {
        // Arrange
        var order = Order.Create(_otherUserId);

        // Act
        var result = order.AddLine(_lineItem1);

        // Assert
        result.IsSuccess.Should().BeTrue();
        order.Lines.Should().ContainSingle();
        order.TotalPrice.Amount.Should().Be(10);
    }

    [Fact]
    public void AddLine_ShouldFail_WhenLineItemIsNull()
    {
        // Arrange
        var order = Order.Create(_externalUserId);

        // Act
        var result = order.AddLine(null!);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(LineItemErrors.IsNull);
    }

    [Fact]
    public void AddLines_ShouldAddMultipleItemsAndRecalculateTotal_WhenItemsAreValid()
    {
        // Arrange
        var order = Order.Create(_externalUserId);
        var items = new[] { _lineItem1, _lineItem2 };

        // Act
        var result = order.AddLines(items);

        // Assert
        result.IsSuccess.Should().BeTrue();
        order.Lines.Should().BeEquivalentTo(items);
        order.TotalPrice.Amount.Should().Be(40); // 1*10 + 2*15
    }

    [Fact]
    public void AddLines_ShouldFail_WhenItemsIsNullOrEmpty()
    {
        // Arrange
        var order = Order.Create(_externalUserId);

        // Act
        var resultNull = order.AddLines(null!);
        var resultEmpty = order.AddLines(Array.Empty<LineItem>());

        // Assert
        resultNull.IsFailure.Should().BeTrue();
        resultEmpty.Error.Should().Be(LineItemErrors.IsNull);
        resultEmpty.IsFailure.Should().BeTrue();
        resultNull.Error.Should().Be(LineItemErrors.IsNull);
    }

    [Fact]
    public void Submit_ShouldSucceed_WhenOrderIsDraftAndHasItems()
    {
        // Arrange
        var order = Order.Create(_externalUserId);
        order.AddLine(_lineItem1);

        // Act
        var result = order.Submit();

        // Assert
        result.IsSuccess.Should().BeTrue();
        order.Status.Should().Be(OrderStatus.Submitted);
    }

    [Fact]
    public void Submit_ShouldFail_WhenOrderIsAlreadySubmitted()
    {
        // Arrange
        var order = Order.Create(_externalUserId);
        order.AddLine(_lineItem1);
        order.Submit();

        // Act
        var result = order.Submit();

        // Assert
        result.IsFailure.Should().BeTrue();
        order.Status.Should().Be(OrderStatus.Submitted);
    }

    [Fact]
    public void Submit_ShouldFail_WhenOrderIsEmpty()
    {
        // Arrange
        var order = Order.Create(_externalUserId);

        // Act
        var result = order.Submit();

        // Assert
        result.IsFailure.Should().BeTrue();
        order.Status.Should().Be(OrderStatus.Draft);
    }
}
