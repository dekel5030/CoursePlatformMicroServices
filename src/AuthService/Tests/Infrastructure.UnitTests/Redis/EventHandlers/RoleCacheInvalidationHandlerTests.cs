using Domain.Permissions;
using Domain.Roles;
using Domain.Roles.Events;
using FluentAssertions;
using Infrastructure.Redis.EventCollector;
using Infrastructure.Redis.EventHandlers;
using Kernel.Auth.AuthTypes;
using Moq;
using Xunit;

namespace Infrastructure.UnitTests.Redis.EventHandlers;

/// <summary>
/// Unit tests for RoleCacheInvalidationHandler.
/// Verifies that domain events trigger cache invalidation by marking roles for refresh.
/// </summary>
public class RoleCacheInvalidationHandlerTests
{
    private readonly Mock<IRoleEventsCollector> _mockCollector;
    private readonly RoleCacheInvalidationHandler _sut;

    public RoleCacheInvalidationHandlerTests()
    {
        _mockCollector = new Mock<IRoleEventsCollector>();
        _sut = new RoleCacheInvalidationHandler(_mockCollector.Object);
    }

    [Fact]
    public async Task Handle_RoleCreatedDomainEvent_ShouldMarkRoleForRefresh()
    {
        // Arrange
        var roleResult = Role.Create("Admin");
        var role = roleResult.Value;
        var domainEvent = new RoleCreatedDomainEvent(role);
        var cancellationToken = CancellationToken.None;

        // Act
        await _sut.Handle(domainEvent, cancellationToken);

        // Assert
        _mockCollector.Verify(
            c => c.MarkRoleForRefresh(role.Name),
            Times.Once);
    }

    [Fact]
    public async Task Handle_RolePermissionAddedDomainEvent_ShouldMarkRoleForRefresh()
    {
        // Arrange
        var roleResult = Role.Create("Moderator");
        var role = roleResult.Value;
        var permission = Permission.CreateForRole(
            ActionType.Read,
            ResourceType.Course,
            ResourceId.Create("*"));
        var domainEvent = new RolePermissionAddedDomainEvent(role, permission);
        var cancellationToken = CancellationToken.None;

        // Act
        await _sut.Handle(domainEvent, cancellationToken);

        // Assert
        _mockCollector.Verify(
            c => c.MarkRoleForRefresh(role.Name),
            Times.Once);
    }

    [Fact]
    public async Task Handle_RolePermissionRemovedDomainEvent_ShouldMarkRoleForRefresh()
    {
        // Arrange
        var roleResult = Role.Create("User");
        var role = roleResult.Value;
        var permission = Permission.CreateForRole(
            ActionType.Delete,
            ResourceType.Lesson,
            ResourceId.Create("123"));
        var domainEvent = new RolePermissionRemovedDomainEvent(role, permission);
        var cancellationToken = CancellationToken.None;

        // Act
        await _sut.Handle(domainEvent, cancellationToken);

        // Assert
        _mockCollector.Verify(
            c => c.MarkRoleForRefresh(role.Name),
            Times.Once);
    }

    [Theory]
    [InlineData("Admin")]
    [InlineData("User")]
    [InlineData("Moderator")]
    [InlineData("Guest")]
    public async Task Handle_RoleCreatedDomainEvent_WithDifferentRoleNames_ShouldMarkCorrectRole(string roleName)
    {
        // Arrange
        var roleResult = Role.Create(roleName);
        var role = roleResult.Value;
        var domainEvent = new RoleCreatedDomainEvent(role);
        var cancellationToken = CancellationToken.None;

        // Act
        await _sut.Handle(domainEvent, cancellationToken);

        // Assert
        _mockCollector.Verify(
            c => c.MarkRoleForRefresh(roleName),
            Times.Once);
    }

    [Fact]
    public async Task Handle_RoleCreatedDomainEvent_ShouldCompleteSuccessfully()
    {
        // Arrange
        var roleResult = Role.Create("Admin");
        var role = roleResult.Value;
        var domainEvent = new RoleCreatedDomainEvent(role);
        var cancellationToken = CancellationToken.None;

        // Act
        var act = async () => await _sut.Handle(domainEvent, cancellationToken);

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task Handle_RolePermissionAddedDomainEvent_ShouldCompleteSuccessfully()
    {
        // Arrange
        var roleResult = Role.Create("Editor");
        var role = roleResult.Value;
        var permission = Permission.CreateForRole(
            ActionType.Update,
            ResourceType.Course,
            ResourceId.Create("*"));
        var domainEvent = new RolePermissionAddedDomainEvent(role, permission);
        var cancellationToken = CancellationToken.None;

        // Act
        var act = async () => await _sut.Handle(domainEvent, cancellationToken);

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task Handle_RolePermissionRemovedDomainEvent_ShouldCompleteSuccessfully()
    {
        // Arrange
        var roleResult = Role.Create("Viewer");
        var role = roleResult.Value;
        var permission = Permission.CreateForRole(
            ActionType.Read,
            ResourceType.User,
            ResourceId.Create("*"));
        var domainEvent = new RolePermissionRemovedDomainEvent(role, permission);
        var cancellationToken = CancellationToken.None;

        // Act
        var act = async () => await _sut.Handle(domainEvent, cancellationToken);

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task Handle_MultipleRoleCreatedEvents_ShouldMarkEachRoleOnce()
    {
        // Arrange
        var roleNames = new[] { "Admin", "User", "Moderator" };
        var cancellationToken = CancellationToken.None;

        // Act
        foreach (var roleName in roleNames)
        {
            var roleResult = Role.Create(roleName);
            var role = roleResult.Value;
            var domainEvent = new RoleCreatedDomainEvent(role);
            await _sut.Handle(domainEvent, cancellationToken);
        }

        // Assert
        foreach (var roleName in roleNames)
        {
            _mockCollector.Verify(
                c => c.MarkRoleForRefresh(roleName),
                Times.Once);
        }
    }

    [Fact]
    public async Task Handle_RolePermissionAddedDomainEvent_WithDifferentPermissions_ShouldAlwaysMarkRole()
    {
        // Arrange
        var roleResult = Role.Create("Admin");
        var role = roleResult.Value;
        var cancellationToken = CancellationToken.None;

        var permissions = new[]
        {
            Permission.CreateForRole(ActionType.Create, ResourceType.Course, ResourceId.Create("*")),
            Permission.CreateForRole(ActionType.Read, ResourceType.User, ResourceId.Create("123")),
            Permission.CreateForRole(ActionType.Delete, ResourceType.Lesson, ResourceId.Create("*"))
        };

        // Act
        foreach (var permission in permissions)
        {
            var domainEvent = new RolePermissionAddedDomainEvent(role, permission);
            await _sut.Handle(domainEvent, cancellationToken);
        }

        // Assert
        _mockCollector.Verify(
            c => c.MarkRoleForRefresh(role.Name),
            Times.Exactly(permissions.Length));
    }

    [Fact]
    public async Task Handle_RolePermissionRemovedDomainEvent_WithDifferentPermissions_ShouldAlwaysMarkRole()
    {
        // Arrange
        var roleResult = Role.Create("Moderator");
        var role = roleResult.Value;
        var cancellationToken = CancellationToken.None;

        var permissions = new[]
        {
            Permission.CreateForRole(ActionType.Update, ResourceType.Course, ResourceId.Create("456")),
            Permission.CreateForRole(ActionType.Delete, ResourceType.User, ResourceId.Create("*"))
        };

        // Act
        foreach (var permission in permissions)
        {
            var domainEvent = new RolePermissionRemovedDomainEvent(role, permission);
            await _sut.Handle(domainEvent, cancellationToken);
        }

        // Assert
        _mockCollector.Verify(
            c => c.MarkRoleForRefresh(role.Name),
            Times.Exactly(permissions.Length));
    }

    [Fact]
    public async Task Handle_SameRoleMultipleEvents_ShouldMarkRoleMultipleTimes()
    {
        // Arrange
        var roleResult = Role.Create("Admin");
        var role = roleResult.Value;
        var permission = Permission.CreateForRole(
            ActionType.Read,
            ResourceType.Course,
            ResourceId.Create("*"));
        var cancellationToken = CancellationToken.None;

        // Act
        await _sut.Handle(new RoleCreatedDomainEvent(role), cancellationToken);
        await _sut.Handle(new RolePermissionAddedDomainEvent(role, permission), cancellationToken);
        await _sut.Handle(new RolePermissionRemovedDomainEvent(role, permission), cancellationToken);

        // Assert
        _mockCollector.Verify(
            c => c.MarkRoleForRefresh(role.Name),
            Times.Exactly(3),
            "Each event should independently mark the role for refresh");
    }

    [Fact]
    public async Task Handle_WithCancellationToken_ShouldNotThrow()
    {
        // Arrange
        var roleResult = Role.Create("TestRole");
        var role = roleResult.Value;
        var domainEvent = new RoleCreatedDomainEvent(role);
        var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;

        // Act
        var act = async () => await _sut.Handle(domainEvent, cancellationToken);

        // Assert
        await act.Should().NotThrowAsync();
    }
}
