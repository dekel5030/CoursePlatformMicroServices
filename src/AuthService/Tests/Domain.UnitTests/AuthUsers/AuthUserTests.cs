using Domain.AuthUsers;
using Domain.AuthUsers.Events;
using Domain.Roles;
using FluentAssertions;
using Xunit;

namespace Domain.UnitTests.AuthUsers;

/// <summary>
/// Unit tests for the AuthUser domain entity.
/// Tests cover entity creation, domain event dispatching, and business logic.
/// </summary>
public class AuthUserTests
{
    /// <summary>
    /// Verifies that AuthUser.Create successfully creates a user with valid data and assigns the initial role.
    /// </summary>
    [Fact]
    public void Create_WithValidData_ShouldCreateAuthUserSuccessfully()
    {
        // Arrange
        var email = "test@example.com";
        var userName = "testuser";
        var role = Role.Create("User");

        // Act
        var authUser = AuthUser.Create(email, userName, role);

        // Assert
        authUser.Should().NotBeNull();
        authUser.Id.Should().NotBeEmpty();
        authUser.Email.Should().Be(email);
        authUser.UserName.Should().Be(userName);
        authUser.Roles.Should().ContainSingle();
        authUser.Roles.Should().Contain(role);
    }

    /// <summary>
    /// Verifies that when username is empty, the email is used as the username.
    /// </summary>
    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Create_WithEmptyUsername_ShouldUseEmailAsUsername(string? userName)
    {
        // Arrange
        var email = "test@example.com";
        var role = Role.Create("User");

        // Act
        var authUser = AuthUser.Create(email, userName!, role);

        // Assert
        authUser.UserName.Should().Be(email);
    }

    /// <summary>
    /// Verifies that AuthUser.Create raises a UserRegisteredDomainEvent when a user is created.
    /// This demonstrates domain event dispatching which is critical for event-driven architecture.
    /// </summary>
    [Fact]
    public void Create_ShouldRaiseUserRegisteredDomainEvent()
    {
        // Arrange
        var email = "test@example.com";
        var userName = "testuser";
        var role = Role.Create("User");

        // Act
        var authUser = AuthUser.Create(email, userName, role);

        // Assert
        authUser.DomainEvents.Should().ContainSingle();
        var domainEvent = authUser.DomainEvents.First();
        domainEvent.Should().BeOfType<UserRegisteredDomainEvent>();
        
        var userRegisteredEvent = (UserRegisteredDomainEvent)domainEvent;
        userRegisteredEvent.AuthUserId.Should().Be(authUser.Id);
        userRegisteredEvent.Email.Should().Be(email);
        userRegisteredEvent.RegisteredAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    /// <summary>
    /// Verifies that multiple users can be created with different emails.
    /// </summary>
    [Theory]
    [InlineData("user1@example.com", "user1")]
    [InlineData("user2@example.com", "user2")]
    [InlineData("admin@example.com", "admin")]
    public void Create_WithDifferentEmails_ShouldCreateDifferentUsers(string email, string userName)
    {
        // Arrange
        var role = Role.Create("User");

        // Act
        var authUser = AuthUser.Create(email, userName, role);

        // Assert
        authUser.Email.Should().Be(email);
        authUser.UserName.Should().Be(userName);
        authUser.Id.Should().NotBeEmpty();
    }

    /// <summary>
    /// Verifies that each created user has a unique GUID identifier.
    /// </summary>
    [Fact]
    public void Create_ShouldGenerateUniqueIds()
    {
        // Arrange
        var role = Role.Create("User");

        // Act
        var user1 = AuthUser.Create("user1@example.com", "user1", role);
        var user2 = AuthUser.Create("user2@example.com", "user2", role);

        // Assert
        user1.Id.Should().NotBe(user2.Id);
    }

    /// <summary>
    /// Verifies that the roles collection is read-only and properly initialized.
    /// </summary>
    [Fact]
    public void Roles_ShouldBeReadOnlyCollection()
    {
        // Arrange
        var email = "test@example.com";
        var userName = "testuser";
        var role = Role.Create("User");

        // Act
        var authUser = AuthUser.Create(email, userName, role);

        // Assert
        authUser.Roles.Should().NotBeNull();
        authUser.Roles.Should().BeAssignableTo<IReadOnlyCollection<Role>>();
    }

    /// <summary>
    /// Verifies that clearing domain events removes all raised events.
    /// </summary>
    [Fact]
    public void ClearDomainEvents_ShouldRemoveAllEvents()
    {
        // Arrange
        var role = Role.Create("User");
        var authUser = AuthUser.Create("test@example.com", "testuser", role);

        // Act
        authUser.ClearDomainEvents();

        // Assert
        authUser.DomainEvents.Should().BeEmpty();
    }
}
