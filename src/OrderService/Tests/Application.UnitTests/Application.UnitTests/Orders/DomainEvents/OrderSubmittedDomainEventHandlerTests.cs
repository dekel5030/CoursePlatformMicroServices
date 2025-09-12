using Application.Orders.DomainEvents;
using Application.Abstractions.Messaging;
using Domain.Orders.Events;
using Domain.Orders.Primitives;
using Domain.Users.Primitives;
using Moq;
using Xunit;
using Orders.Contracts;
using OrderStatus = Domain.Orders.Primitives.OrderStatus;

namespace Application.UnitTests.Orders.DomainEvents;

public class OrderSubmittedDomainEventHandlerTests
{
    [Fact]
    public async Task Handle_ShouldPublishIntegrationEvent_WithCorrectData()
    {
        // Arrange
        var publisherMock = new Mock<IEventPublisher>();
        var handler = new OrderSubmittedDomainEventHandler(publisherMock.Object);

        var orderId = new OrderId(Guid.NewGuid());
        long version = 1L;
        var status = OrderStatus.Submitted;

        var domainEvent = new OrderSubmittedDomainEvent(orderId, version, status);

        // Act
        await handler.Handle(domainEvent, default);

        // Assert
        publisherMock.Verify(p => p.Publish(
            It.Is<OrderSubmitted>(ie =>
                ie.OrderId == orderId.Value.ToString() &&
                ie.EntityVersion == version &&
                ie.Status == status.ToString()
            ),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
