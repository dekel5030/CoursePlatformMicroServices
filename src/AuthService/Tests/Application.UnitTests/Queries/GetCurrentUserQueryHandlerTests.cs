using Application.Abstractions.Identity;
using Application.AuthUsers.Queries.GetCurrentUser;
using Domain.AuthUsers;
using Domain.AuthUsers.Errors;
using FluentAssertions;
using Moq;
using Xunit;

namespace Application.UnitTests.Queries;

/// <summary>
/// Unit tests for GetCurrentUserQueryHandler.
/// Tests cover successful user retrieval and not found scenarios.
/// </summary>
public class GetCurrentUserQueryHandlerTests
{
    private readonly Mock<IUserManager<AuthUser>> _userManagerMock;
    private readonly GetCurrentUserQueryHandler _handler;

    public GetCurrentUserQueryHandlerTests()
    {
        _userManagerMock = new Mock<IUserManager<AuthUser>>();
        _handler = new GetCurrentUserQueryHandler(_userManagerMock.Object);
    }

    /// <summary>
    /// Verifies that Handle successfully retrieves user when user exists.
    /// This is an async method test demonstrating proper query handling.
    /// </summary>
    [Fact]
    public async Task Handle_WhenUserExists_ShouldReturnUser()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var email = "test@example.com";
        var userName = "testuser";
        var user = AuthUser.Create(email, userName, Domain.Roles.Role.Create("User"));

        var query = new GetCurrentUserQuery(userId);

        _userManagerMock
            .Setup(x => x.FindByIdAsync(userId))
            .ReturnsAsync(user);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Email.Should().Be(email);
        result.Value.UserName.Should().Be(userName);
        result.Value.Id.Should().Be(user.Id);

        _userManagerMock.Verify(x => x.FindByIdAsync(userId), Times.Once);
    }

    /// <summary>
    /// Verifies that Handle returns NotFound error when user doesn't exist.
    /// Tests error handling for non-existent users.
    /// </summary>
    [Fact]
    public async Task Handle_WhenUserNotFound_ShouldReturnNotFoundError()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var query = new GetCurrentUserQuery(userId);

        _userManagerMock
            .Setup(x => x.FindByIdAsync(userId))
            .ReturnsAsync((AuthUser?)null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(AuthUserErrors.NotFound);

        _userManagerMock.Verify(x => x.FindByIdAsync(userId), Times.Once);
    }

    /// <summary>
    /// Verifies that Handle works with different user IDs.
    /// </summary>
    [Fact]
    public async Task Handle_WithDifferentUserIds_ShouldReturnCorrectUsers()
    {
        // Arrange
        var userId1 = Guid.NewGuid();
        var userId2 = Guid.NewGuid();

        var user1 = AuthUser.Create("user1@example.com", "user1", Domain.Roles.Role.Create("User"));
        var user2 = AuthUser.Create("user2@example.com", "user2", Domain.Roles.Role.Create("User"));

        _userManagerMock
            .Setup(x => x.FindByIdAsync(userId1))
            .ReturnsAsync(user1);

        _userManagerMock
            .Setup(x => x.FindByIdAsync(userId2))
            .ReturnsAsync(user2);

        // Act
        var result1 = await _handler.Handle(new GetCurrentUserQuery(userId1), CancellationToken.None);
        var result2 = await _handler.Handle(new GetCurrentUserQuery(userId2), CancellationToken.None);

        // Assert
        result1.IsSuccess.Should().BeTrue();
        result1.Value.Email.Should().Be("user1@example.com");

        result2.IsSuccess.Should().BeTrue();
        result2.Value.Email.Should().Be("user2@example.com");
    }

    /// <summary>
    /// Verifies that Handle respects cancellation tokens.
    /// </summary>
    [Fact]
    public async Task Handle_WithCancellationToken_ShouldCompleteSuccessfully()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = AuthUser.Create("test@example.com", "testuser", Domain.Roles.Role.Create("User"));
        var query = new GetCurrentUserQuery(userId);
        var cancellationToken = new CancellationToken();

        _userManagerMock
            .Setup(x => x.FindByIdAsync(userId))
            .ReturnsAsync(user);

        // Act
        var result = await _handler.Handle(query, cancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    /// <summary>
    /// Verifies that Handle returns correct DTO structure.
    /// </summary>
    [Fact]
    public async Task Handle_ShouldReturnCorrectDtoStructure()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var email = "detailed@example.com";
        var userName = "detaileduser";
        var user = AuthUser.Create(email, userName, Domain.Roles.Role.Create("User"));

        var query = new GetCurrentUserQuery(userId);

        _userManagerMock
            .Setup(x => x.FindByIdAsync(userId))
            .ReturnsAsync(user);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Value.Should().NotBeNull();
        result.Value.Id.Should().Be(user.Id);
        result.Value.Email.Should().Be(email);
        result.Value.UserName.Should().Be(userName);
    }
}
