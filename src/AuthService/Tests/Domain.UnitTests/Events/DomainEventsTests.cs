using Domain.AuthUsers.Events;
using Domain.AuthUsers.Primitives;
using Domain.Roles.Primitives;
using FluentAssertions;
using Xunit;

namespace Domain.UnitTests.Events;

/// <summary>
/// Unit tests for domain events.
/// Tests verify that domain events are properly created with correct data.
/// </summary>
public class DomainEventsTests
{
    #region UserRegisteredDomainEvent Tests

    /// <summary>
    /// Verifies that UserRegisteredDomainEvent is created with correct properties.
    /// </summary>
    [Fact]
    public void UserRegisteredDomainEvent_ShouldCreateWithCorrectProperties()
    {
        // Arrange
        var authUserId = Guid.NewGuid();
        var email = "test@example.com";
        var registeredAt = DateTime.UtcNow;

        // Act
        var domainEvent = new UserRegisteredDomainEvent(authUserId, email, registeredAt);

        // Assert
        domainEvent.AuthUserId.Should().Be(authUserId);
        domainEvent.Email.Should().Be(email);
        domainEvent.RegisteredAt.Should().Be(registeredAt);
    }

    /// <summary>
    /// Verifies that UserRegisteredDomainEvent with different emails creates different events.
    /// </summary>
    [Theory]
    [InlineData("user1@example.com")]
    [InlineData("user2@example.com")]
    [InlineData("admin@example.com")]
    public void UserRegisteredDomainEvent_WithDifferentEmails_ShouldCreateDistinctEvents(string email)
    {
        // Arrange
        var authUserId = Guid.NewGuid();
        var registeredAt = DateTime.UtcNow;

        // Act
        var domainEvent = new UserRegisteredDomainEvent(authUserId, email, registeredAt);

        // Assert
        domainEvent.Email.Should().Be(email);
    }

    /// <summary>
    /// Verifies that UserRegisteredDomainEvent records are equal when all properties match.
    /// </summary>
    [Fact]
    public void UserRegisteredDomainEvent_WithSameProperties_ShouldBeEqual()
    {
        // Arrange
        var authUserId = Guid.NewGuid();
        var email = "test@example.com";
        var registeredAt = DateTime.UtcNow;

        // Act
        var event1 = new UserRegisteredDomainEvent(authUserId, email, registeredAt);
        var event2 = new UserRegisteredDomainEvent(authUserId, email, registeredAt);

        // Assert
        event1.Should().Be(event2);
    }

    /// <summary>
    /// Verifies that UserRegisteredDomainEvent records are not equal when properties differ.
    /// </summary>
    [Fact]
    public void UserRegisteredDomainEvent_WithDifferentProperties_ShouldNotBeEqual()
    {
        // Arrange
        var authUserId1 = Guid.NewGuid();
        var authUserId2 = Guid.NewGuid();
        var email = "test@example.com";
        var registeredAt = DateTime.UtcNow;

        // Act
        var event1 = new UserRegisteredDomainEvent(authUserId1, email, registeredAt);
        var event2 = new UserRegisteredDomainEvent(authUserId2, email, registeredAt);

        // Assert
        event1.Should().NotBe(event2);
    }

    #endregion

    #region RoleAssignedDomainEvent Tests

    /// <summary>
    /// Verifies that RoleAssignedDomainEvent is created with correct properties.
    /// </summary>
    [Fact]
    public void RoleAssignedDomainEvent_ShouldCreateWithCorrectProperties()
    {
        // Arrange
        var authUserId = new AuthUserId(Guid.NewGuid());
        var roleId = new RoleId(1);
        var roleName = "Administrator";
        var assignedAt = DateTime.UtcNow;

        // Act
        var domainEvent = new RoleAssignedDomainEvent(authUserId, roleId, roleName, assignedAt);

        // Assert
        domainEvent.AuthUserId.Should().Be(authUserId);
        domainEvent.RoleId.Should().Be(roleId);
        domainEvent.RoleName.Should().Be(roleName);
        domainEvent.AssignedAt.Should().Be(assignedAt);
    }

    /// <summary>
    /// Verifies that RoleAssignedDomainEvent with different role names creates different events.
    /// </summary>
    [Theory]
    [InlineData("User")]
    [InlineData("Administrator")]
    [InlineData("Moderator")]
    public void RoleAssignedDomainEvent_WithDifferentRoleNames_ShouldCreateDistinctEvents(string roleName)
    {
        // Arrange
        var authUserId = new AuthUserId(Guid.NewGuid());
        var roleId = new RoleId(1);
        var assignedAt = DateTime.UtcNow;

        // Act
        var domainEvent = new RoleAssignedDomainEvent(authUserId, roleId, roleName, assignedAt);

        // Assert
        domainEvent.RoleName.Should().Be(roleName);
    }

    /// <summary>
    /// Verifies that RoleAssignedDomainEvent records are equal when all properties match.
    /// </summary>
    [Fact]
    public void RoleAssignedDomainEvent_WithSameProperties_ShouldBeEqual()
    {
        // Arrange
        var authUserId = new AuthUserId(Guid.NewGuid());
        var roleId = new RoleId(1);
        var roleName = "User";
        var assignedAt = DateTime.UtcNow;

        // Act
        var event1 = new RoleAssignedDomainEvent(authUserId, roleId, roleName, assignedAt);
        var event2 = new RoleAssignedDomainEvent(authUserId, roleId, roleName, assignedAt);

        // Assert
        event1.Should().Be(event2);
    }

    /// <summary>
    /// Verifies that RoleAssignedDomainEvent records are not equal when properties differ.
    /// </summary>
    [Fact]
    public void RoleAssignedDomainEvent_WithDifferentProperties_ShouldNotBeEqual()
    {
        // Arrange
        var authUserId = new AuthUserId(Guid.NewGuid());
        var roleId1 = new RoleId(1);
        var roleId2 = new RoleId(2);
        var roleName = "User";
        var assignedAt = DateTime.UtcNow;

        // Act
        var event1 = new RoleAssignedDomainEvent(authUserId, roleId1, roleName, assignedAt);
        var event2 = new RoleAssignedDomainEvent(authUserId, roleId2, roleName, assignedAt);

        // Assert
        event1.Should().NotBe(event2);
    }

    #endregion
}
