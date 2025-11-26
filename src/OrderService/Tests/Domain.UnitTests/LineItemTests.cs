using Domain.Orders;
using Domain.Orders.Errors;
using Domain.Products.Primitives;
using FluentAssertions;
using Kernel;
using SharedKernel;
using Xunit;

namespace Domain.UnitTests;

public class LineItemTests
{
    private readonly ExternalProductId _productId = new("prod-1");
    private readonly Money _validPrice = new(10, "ILS");
    private readonly string _validName = "Test Product";

    [Fact]
    public void Create_ShouldSucceed_WhenAllArgumentsAreValid()
    {
        // Arrange
        decimal quantity = 2;

        // Act
        var result = LineItem.Create(_productId, quantity, _validName, _validPrice);
        LineItem item = result.Value;

        // Assert
        result.IsSuccess.Should().BeTrue();
        item.ExternalProductId.Should().Be(_productId);
        item.Quantity.Should().Be(quantity);
        item.Name.Should().Be(_validName);
        item.UnitPrice.Should().Be(_validPrice);
        item.TotalPrice.Amount.Should().Be(_validPrice.Amount * quantity);
        item.Id.Should().NotBeNull();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Create_ShouldFail_WhenQuantityIsZeroOrNegative(decimal quantity)
    {
        // Act
        var result = LineItem.Create(_productId, quantity, _validName, _validPrice);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(LineItemErrors.InvalidQuantity);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Create_ShouldFail_WhenNameIsNullOrWhitespace(string? name)
    {
        // Act
        var result = LineItem.Create(_productId, 1, name!, _validPrice);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(LineItemErrors.IsNull);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Create_ShouldFail_WhenUnitPriceIsNegative(decimal price)
    {
        // Act
        var result = LineItem.Create(_productId, 1, _validName, new Money(price, "ILS"));

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(LineItemErrors.InvalidPrice);
    }

    [Fact]
    public void TotalPrice_ShouldBeUnitPriceTimesQuantity()
    {
        // Arrange
        var result = LineItem.Create(_productId, 3, _validName, new Money(5, "ILS"));

        // Act
        var item = result.Value;

        // Assert
        item.TotalPrice.Amount.Should().Be(15);
        item.TotalPrice.Currency.Should().Be("ILS");
    }
}

