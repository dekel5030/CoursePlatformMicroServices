using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.AuthUsers.Events;
using Auth.Contracts.Events;
using Domain.AuthUsers.Events;
using FluentAssertions;
using Moq;
using Xunit;

namespace Application.UnitTests.Events;

/// <summary>
/// Unit tests for UserRegisteredDomainEventHandler.
/// Tests verify that domain events are properly handled and integration events are published.
/// This demonstrates domain event dispatching which is critical for event-driven architecture.
/// </summary>
public class UserRegisteredDomainEventHandlerTests
{
    private readonly Mock<IReadDbContext> _readDbContextMock;
    private readonly Mock<IEventPublisher> _eventPublisherMock;
    private readonly UserRegisteredDomainEventHandler _handler;

    public UserRegisteredDomainEventHandlerTests()
    {
        _readDbContextMock = new Mock<IReadDbContext>();
        _eventPublisherMock = new Mock<IEventPublisher>();
        _handler = new UserRegisteredDomainEventHandler(
            _readDbContextMock.Object,
            _eventPublisherMock.Object);
    }

    /// <summary>
    /// Verifies that Handle publishes the correct integration event when domain event is received.
    /// This is an async method test demonstrating proper event handling and publishing.
    /// </summary>
    [Fact]
    public async Task Handle_ShouldPublishIntegrationEvent()
    {
        // Arrange
        var authUserId = Guid.NewGuid();
        var email = "test@example.com";
        var registeredAt = DateTime.UtcNow;

        var domainEvent = new UserRegisteredDomainEvent(authUserId, email, registeredAt);

        UserRegistered? publishedEvent = null;
        _eventPublisherMock
            .Setup(x => x.PublishAsync(It.IsAny<UserRegistered>(), It.IsAny<CancellationToken>()))
            .Callback<object, CancellationToken>((evt, _) => publishedEvent = evt as UserRegistered)
            .Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(domainEvent, CancellationToken.None);

        // Assert
        _eventPublisherMock.Verify(
            x => x.PublishAsync(It.IsAny<UserRegistered>(), It.IsAny<CancellationToken>()),
            Times.Once);

        publishedEvent.Should().NotBeNull();
        publishedEvent!.UserId.Should().Be(authUserId.ToString());
        publishedEvent.AuthUserId.Should().Be(authUserId.ToString());
        publishedEvent.Email.Should().Be(email);
        publishedEvent.RegisteredAt.Should().Be(registeredAt);
    }

    /// <summary>
    /// Verifies that Handle maps domain event properties correctly to integration event.
    /// </summary>
    [Theory]
    [InlineData("user1@example.com")]
    [InlineData("user2@example.com")]
    [InlineData("admin@example.com")]
    public async Task Handle_ShouldMapDomainEventPropertiesCorrectly(string email)
    {
        // Arrange
        var authUserId = Guid.NewGuid();
        var registeredAt = DateTime.UtcNow;

        var domainEvent = new UserRegisteredDomainEvent(authUserId, email, registeredAt);

        UserRegistered? publishedEvent = null;
        _eventPublisherMock
            .Setup(x => x.PublishAsync(It.IsAny<UserRegistered>(), It.IsAny<CancellationToken>()))
            .Callback<object, CancellationToken>((evt, _) => publishedEvent = evt as UserRegistered)
            .Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(domainEvent, CancellationToken.None);

        // Assert
        publishedEvent.Should().NotBeNull();
        publishedEvent!.Email.Should().Be(email);
        publishedEvent.UserId.Should().Be(authUserId.ToString());
        publishedEvent.AuthUserId.Should().Be(authUserId.ToString());
    }

    /// <summary>
    /// Verifies that Handle respects cancellation tokens.
    /// </summary>
    [Fact]
    public async Task Handle_WithCancellationToken_ShouldPassThroughToPublisher()
    {
        // Arrange
        var authUserId = Guid.NewGuid();
        var email = "test@example.com";
        var registeredAt = DateTime.UtcNow;

        var domainEvent = new UserRegisteredDomainEvent(authUserId, email, registeredAt);
        var cancellationToken = new CancellationToken();

        // Act
        await _handler.Handle(domainEvent, cancellationToken);

        // Assert
        _eventPublisherMock.Verify(
            x => x.PublishAsync(It.IsAny<UserRegistered>(), cancellationToken),
            Times.Once);
    }

    /// <summary>
    /// Verifies that Handle can process multiple domain events sequentially.
    /// </summary>
    [Fact]
    public async Task Handle_CalledMultipleTimes_ShouldPublishMultipleEvents()
    {
        // Arrange
        var domainEvent1 = new UserRegisteredDomainEvent(
            Guid.NewGuid(),
            "user1@example.com",
            DateTime.UtcNow);

        var domainEvent2 = new UserRegisteredDomainEvent(
            Guid.NewGuid(),
            "user2@example.com",
            DateTime.UtcNow);

        // Act
        await _handler.Handle(domainEvent1, CancellationToken.None);
        await _handler.Handle(domainEvent2, CancellationToken.None);

        // Assert
        _eventPublisherMock.Verify(
            x => x.PublishAsync(It.IsAny<UserRegistered>(), It.IsAny<CancellationToken>()),
            Times.Exactly(2));
    }

    /// <summary>
    /// Verifies that Handle creates integration event with matching timestamp.
    /// </summary>
    [Fact]
    public async Task Handle_ShouldPreserveRegistrationTimestamp()
    {
        // Arrange
        var authUserId = Guid.NewGuid();
        var email = "test@example.com";
        var registeredAt = new DateTime(2024, 1, 15, 10, 30, 0, DateTimeKind.Utc);

        var domainEvent = new UserRegisteredDomainEvent(authUserId, email, registeredAt);

        UserRegistered? publishedEvent = null;
        _eventPublisherMock
            .Setup(x => x.PublishAsync(It.IsAny<UserRegistered>(), It.IsAny<CancellationToken>()))
            .Callback<object, CancellationToken>((evt, _) => publishedEvent = evt as UserRegistered)
            .Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(domainEvent, CancellationToken.None);

        // Assert
        publishedEvent.Should().NotBeNull();
        publishedEvent!.RegisteredAt.Should().Be(registeredAt);
    }
}
