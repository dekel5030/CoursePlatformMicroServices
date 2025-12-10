using Application.Abstractions.Messaging;
using Auth.Contracts.Redis.Events;
using FluentAssertions;
using Infrastructure.Redis.EventCollector;
using Moq;
using Xunit;

namespace Infrastructure.UnitTests.Redis.EventCollector;

/// <summary>
/// Unit tests for RoleEventsCollector.
/// Verifies that role changes are collected and published as events when flushed.
/// </summary>
public class RoleEventsCollectorTests
{
    private readonly Mock<IEventPublisher> _mockPublisher;
    private readonly RoleEventsCollector _sut;

    public RoleEventsCollectorTests()
    {
        _mockPublisher = new Mock<IEventPublisher>();
        _sut = new RoleEventsCollector(_mockPublisher.Object);
    }

    [Fact]
    public void MarkRoleForRefresh_WithSingleRole_ShouldCollectRole()
    {
        // Arrange
        var roleName = "Admin";

        // Act
        _sut.MarkRoleForRefresh(roleName);

        // Assert - Verification will happen on FlushAsync
        _mockPublisher.Verify(
            p => p.PublishAsync(It.IsAny<RolePermissionsChangedEvent>(), It.IsAny<CancellationToken>()),
            Times.Never,
            "Events should not be published until FlushAsync is called");
    }

    [Fact]
    public async Task FlushAsync_WithSingleMarkedRole_ShouldPublishEvent()
    {
        // Arrange
        var roleName = "Admin";
        var cancellationToken = CancellationToken.None;
        _sut.MarkRoleForRefresh(roleName);

        _mockPublisher
            .Setup(p => p.PublishAsync(It.IsAny<RolePermissionsChangedEvent>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _sut.FlushAsync(cancellationToken);

        // Assert
        _mockPublisher.Verify(
            p => p.PublishAsync(
                It.Is<RolePermissionsChangedEvent>(e => e.RoleName == roleName),
                cancellationToken),
            Times.Once);
    }

    [Fact]
    public async Task FlushAsync_WithMultipleMarkedRoles_ShouldPublishEventForEachRole()
    {
        // Arrange
        var roleNames = new[] { "Admin", "User", "Moderator" };
        var cancellationToken = CancellationToken.None;

        foreach (var roleName in roleNames)
        {
            _sut.MarkRoleForRefresh(roleName);
        }

        _mockPublisher
            .Setup(p => p.PublishAsync(It.IsAny<RolePermissionsChangedEvent>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _sut.FlushAsync(cancellationToken);

        // Assert
        foreach (var roleName in roleNames)
        {
            _mockPublisher.Verify(
                p => p.PublishAsync(
                    It.Is<RolePermissionsChangedEvent>(e => e.RoleName == roleName),
                    cancellationToken),
                Times.Once);
        }

        _mockPublisher.Verify(
            p => p.PublishAsync(It.IsAny<RolePermissionsChangedEvent>(), It.IsAny<CancellationToken>()),
            Times.Exactly(roleNames.Length));
    }

    [Fact]
    public async Task FlushAsync_ShouldClearCollectedRoles()
    {
        // Arrange
        var roleName = "Admin";
        var cancellationToken = CancellationToken.None;
        _sut.MarkRoleForRefresh(roleName);

        _mockPublisher
            .Setup(p => p.PublishAsync(It.IsAny<RolePermissionsChangedEvent>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _sut.FlushAsync(cancellationToken);
        await _sut.FlushAsync(cancellationToken); // Flush again

        // Assert
        _mockPublisher.Verify(
            p => p.PublishAsync(
                It.Is<RolePermissionsChangedEvent>(e => e.RoleName == roleName),
                cancellationToken),
            Times.Once,
            "Event should only be published once, as collection should be cleared after first flush");
    }

    [Fact]
    public async Task FlushAsync_WithNoMarkedRoles_ShouldNotPublishAnyEvents()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;

        // Act
        await _sut.FlushAsync(cancellationToken);

        // Assert
        _mockPublisher.Verify(
            p => p.PublishAsync(It.IsAny<RolePermissionsChangedEvent>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task MarkRoleForRefresh_WithDuplicateRoleNames_ShouldPublishOnlyOnce()
    {
        // Arrange
        var roleName = "Admin";
        var cancellationToken = CancellationToken.None;

        _mockPublisher
            .Setup(p => p.PublishAsync(It.IsAny<RolePermissionsChangedEvent>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act - Mark the same role multiple times
        _sut.MarkRoleForRefresh(roleName);
        _sut.MarkRoleForRefresh(roleName);
        _sut.MarkRoleForRefresh(roleName);
        await _sut.FlushAsync(cancellationToken);

        // Assert - Should only publish once for the unique role
        _mockPublisher.Verify(
            p => p.PublishAsync(
                It.Is<RolePermissionsChangedEvent>(e => e.RoleName == roleName),
                cancellationToken),
            Times.Once,
            "Duplicate role names should be deduplicated and published only once");
    }

    [Fact]
    public async Task FlushAsync_AfterMultipleMarkAndFlushCycles_ShouldOnlyPublishNewlyMarkedRoles()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        
        _mockPublisher
            .Setup(p => p.PublishAsync(It.IsAny<RolePermissionsChangedEvent>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act - First cycle
        _sut.MarkRoleForRefresh("Admin");
        await _sut.FlushAsync(cancellationToken);

        // Second cycle
        _sut.MarkRoleForRefresh("User");
        await _sut.FlushAsync(cancellationToken);

        // Assert
        _mockPublisher.Verify(
            p => p.PublishAsync(
                It.Is<RolePermissionsChangedEvent>(e => e.RoleName == "Admin"),
                cancellationToken),
            Times.Once,
            "Admin event should be published only in first cycle");

        _mockPublisher.Verify(
            p => p.PublishAsync(
                It.Is<RolePermissionsChangedEvent>(e => e.RoleName == "User"),
                cancellationToken),
            Times.Once,
            "User event should be published only in second cycle");
    }

    [Fact]
    public async Task FlushAsync_WithCancellationToken_ShouldPassTokenToPublisher()
    {
        // Arrange
        var roleName = "Admin";
        var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;
        _sut.MarkRoleForRefresh(roleName);

        CancellationToken? capturedToken = null;
        _mockPublisher
            .Setup(p => p.PublishAsync(It.IsAny<RolePermissionsChangedEvent>(), It.IsAny<CancellationToken>()))
            .Callback<RolePermissionsChangedEvent, CancellationToken>((e, ct) => capturedToken = ct)
            .Returns(Task.CompletedTask);

        // Act
        await _sut.FlushAsync(cancellationToken);

        // Assert
        capturedToken.Should().NotBeNull();
        capturedToken.Should().Be(cancellationToken);
    }

    [Theory]
    [InlineData("Admin")]
    [InlineData("User")]
    [InlineData("Moderator")]
    [InlineData("Guest")]
    [InlineData("SuperAdmin")]
    public async Task MarkRoleForRefresh_WithVariousRoleNames_ShouldPublishCorrectEvent(string roleName)
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        _sut.MarkRoleForRefresh(roleName);

        _mockPublisher
            .Setup(p => p.PublishAsync(It.IsAny<RolePermissionsChangedEvent>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _sut.FlushAsync(cancellationToken);

        // Assert
        _mockPublisher.Verify(
            p => p.PublishAsync(
                It.Is<RolePermissionsChangedEvent>(e => e.RoleName == roleName),
                cancellationToken),
            Times.Once);
    }

    [Fact]
    public async Task MarkRoleForRefresh_MultipleTimes_BetweenFlushes_ShouldAccumulateRoles()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        
        _mockPublisher
            .Setup(p => p.PublishAsync(It.IsAny<RolePermissionsChangedEvent>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        _sut.MarkRoleForRefresh("Admin");
        _sut.MarkRoleForRefresh("User");
        _sut.MarkRoleForRefresh("Moderator");
        _sut.MarkRoleForRefresh("Admin"); // Duplicate
        await _sut.FlushAsync(cancellationToken);

        // Assert - Should publish for 3 unique roles
        _mockPublisher.Verify(
            p => p.PublishAsync(It.IsAny<RolePermissionsChangedEvent>(), It.IsAny<CancellationToken>()),
            Times.Exactly(3));
    }
}
