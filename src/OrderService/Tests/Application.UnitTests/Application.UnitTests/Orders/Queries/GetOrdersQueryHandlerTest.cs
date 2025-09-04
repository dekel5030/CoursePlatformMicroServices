using Application.Abstractions.Data;
using Application.Orders.Queries.GetOrders;
using Domain.Orders;
using Domain.Users.Primitives;
using Domain.Products.Primitives;
using FluentAssertions;
using Moq;
using Moq.EntityFrameworkCore;
using SharedKernel;
using Xunit;
using Application.Orders.Queries.Dtos;

namespace Application.UnitTests.Orders.Queries;

public class GetOrdersQueryHandlerTest
{
    private readonly Mock<IApplicationDbContext> _dbContextMock;
    private readonly GetOrdersQueryHandler _handler;
    private readonly List<Order> _orders;

    public GetOrdersQueryHandlerTest()
    {
        _dbContextMock = new Mock<IApplicationDbContext>();
        _handler = new GetOrdersQueryHandler(_dbContextMock.Object);

        // Initialize the list of orders
        _orders = new List<Order>();
        for (int i = 0; i < 15; i++)
        {
            var externalUserId = new ExternalUserId($"user-{i}");
            var order = Order.Create(externalUserId);
            var lineItem = LineItem.Create(
                new ExternalProductId($"prod-{i}"),
                1,
                $"Product {i}",
                new Money(10 + i, "ILS")
            ).Value;
            order.AddLine(lineItem);
            _orders.Add(order);
        }

        _dbContextMock.Setup(db => db.Orders).ReturnsDbSet(_orders);
    }

    [Fact]
    public async Task Handle_Pagination_ShouldReturnSortedWithoutOverlaps()
    {
        var size = 5;

        var page1 = (await _handler.Handle(new GetOrdersQuery(new(1, size)), default)).Value.Items;
        var page2 = (await _handler.Handle(new GetOrdersQuery(new(2, size)), default)).Value.Items;

        page1.Select(x => x.OrderId).Should().BeInDescendingOrder();
        page2.Select(x => x.OrderId).Should().BeInDescendingOrder();

        page1.Select(x => x.OrderId).Intersect(page2.Select(x => x.OrderId)).Should().BeEmpty();

        var expected = _orders.OrderByDescending(o => o.Id.Value)
                              .Skip(0).Take(size).Select(o => o.Id.Value);
        page1.Select(x => x.OrderId).Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task Handle_WhenPageBeyondLast_ShouldReturnEmpty()
    {
        int total = _orders.Count;
        int size = 5;
        int totalPages = (int)Math.Ceiling(total / (double)size);

        var res = await _handler.Handle(new GetOrdersQuery(new(totalPages + 1, size)), default);
        res.IsSuccess.Should().BeTrue();
        res.Value.Items.Should().BeEmpty();
        res.Value.HasNext.Should().BeFalse();
        res.Value.HasPrevious.Should().BeTrue();
    }

    [Theory]
    [InlineData(0, 1)]
    [InlineData(-10, 1)]
    [InlineData(999, 100)]
    public async Task Handle_ShouldClampPageSize(int requested, int expected)
    {
        var res = await _handler.Handle(new GetOrdersQuery(new(1, requested)), default);
        res.Value.PageSize.Should().Be(expected);
    }

    [Fact]
    public async Task Handle_WhenNoOrders_ShouldReturnEmptyPage()
    {
        _dbContextMock.Setup(db => db.Orders).ReturnsDbSet(Array.Empty<Order>());
        var res = await _handler.Handle(new GetOrdersQuery(new(1, 10)), default);

        res.IsSuccess.Should().BeTrue();
        res.Value.TotalItems.Should().Be(0);
        res.Value.Items.Should().BeEmpty();
        res.Value.HasNext.Should().BeFalse();
        res.Value.HasPrevious.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_ShouldMapDtoFields()
    {
        var first = _orders.OrderByDescending(o => o.Id.Value).First();
        var res = await _handler.Handle(new GetOrdersQuery(new PaginationParams(1, 1)), default);
        var dto = res.Value.Items.Single();

        dto.OrderId.Should().Be(first.Id.Value);
        dto.Total.Should().Be(first.TotalPrice);
        dto.Lines.Count.Should().Be(first.Lines.Count);
        dto.Lines[0].ExternalProductId.Should().Be(first.Lines.First().ExternalProductId.Value);
    }

}

