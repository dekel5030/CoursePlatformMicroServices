using Application.Abstractions.Data;
using Application.AuthUsers.Queries.GetCurrentUser;
using Domain.AuthUsers;
using Domain.AuthUsers.Errors;
using FluentAssertions;
using Kernel.Auth.Abstractions;
using Moq;
using Xunit;

namespace Application.UnitTests.Queries;

/// <summary>
/// Unit tests for GetCurrentUserQueryHandler.
/// Tests cover successful user retrieval and not found scenarios.
/// </summary>
public class GetCurrentUserQueryHandlerTests
{
    private readonly Mock<IUserContext> _userContextMock;
    private readonly Mock<IReadDbContext> _dbContextMock;
    private readonly GetCurrentUserQueryHandler _handler;

    public GetCurrentUserQueryHandlerTests()
    {
        _userContextMock = new Mock<IUserContext>();
        _dbContextMock = new Mock<IReadDbContext>();
        _handler = new GetCurrentUserQueryHandler(_userContextMock.Object, _dbContextMock.Object);
    }

    /// <summary>
    /// Verifies that Handle successfully retrieves user when user exists.
    /// This is an async method test demonstrating proper query handling.
    /// </summary>
    [Fact]
    public async Task Handle_WhenUserExists_ShouldReturnUser()
    {
        // Arrange
        var email = "test@example.com";
        var userName = "testuser";
        var user = AuthUser.Create(email, userName, Domain.Roles.Role.Create("User").Value).Value;
        var userId = user.Id;

        var query = new GetCurrentUserQuery();

        _userContextMock.Setup(x => x.UserId).Returns(userId);
        
        _dbContextMock.Setup(x => x.AuthUsers)
            .Returns(TestHelpers.CreateMockDbSet(new List<AuthUser> { user }).Object);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Email.Should().Be(email);
        result.Value.UserName.Should().Be(userName);
    }

    /// <summary>
    /// Verifies that Handle returns Unauthorized error when user context has no user ID.
    /// Tests error handling for non-authenticated users.
    /// </summary>
    [Fact]
    public async Task Handle_WhenUserIdIsNull_ShouldReturnUnauthorizedError()
    {
        // Arrange
        var query = new GetCurrentUserQuery();

        _userContextMock.Setup(x => x.UserId).Returns((Guid?)null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(AuthUserErrors.Unauthorized);
    }

    /// <summary>
    /// Verifies that Handle works with different user IDs.
    /// </summary>
    [Fact]
    public async Task Handle_WithDifferentUserIds_ShouldReturnCorrectUsers()
    {
        // Arrange
        var user1 = AuthUser.Create("user1@example.com", "user1", Domain.Roles.Role.Create("User").Value).Value;
        var user2 = AuthUser.Create("user2@example.com", "user2", Domain.Roles.Role.Create("User").Value).Value;
        
        var userId1 = user1.Id;
        var userId2 = user2.Id;

        var query1 = new GetCurrentUserQuery();
        var query2 = new GetCurrentUserQuery();

        _userContextMock.SetupSequence(x => x.UserId)
            .Returns(userId1)
            .Returns(userId2);

        _dbContextMock.SetupSequence(x => x.AuthUsers)
            .Returns(TestHelpers.CreateMockDbSet(new List<AuthUser> { user1 }).Object)
            .Returns(TestHelpers.CreateMockDbSet(new List<AuthUser> { user2 }).Object);

        // Act
        var result1 = await _handler.Handle(query1, CancellationToken.None);
        var result2 = await _handler.Handle(query2, CancellationToken.None);

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
        var user = AuthUser.Create("test@example.com", "testuser", Domain.Roles.Role.Create("User").Value).Value;
        var userId = user.Id;
        var query = new GetCurrentUserQuery();
        var cancellationToken = new CancellationToken();

        _userContextMock.Setup(x => x.UserId).Returns(userId);
        _dbContextMock.Setup(x => x.AuthUsers)
            .Returns(TestHelpers.CreateMockDbSet(new List<AuthUser> { user }).Object);

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
        var email = "detailed@example.com";
        var userName = "detaileduser";
        var user = AuthUser.Create(email, userName, Domain.Roles.Role.Create("User").Value).Value;
        var userId = user.Id;

        var query = new GetCurrentUserQuery();

        _userContextMock.Setup(x => x.UserId).Returns(userId);
        _dbContextMock.Setup(x => x.AuthUsers)
            .Returns(TestHelpers.CreateMockDbSet(new List<AuthUser> { user }).Object);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Value.Should().NotBeNull();
        result.Value.Id.Should().Be(user.Id);
        result.Value.Email.Should().Be(email);
        result.Value.UserName.Should().Be(userName);
    }
}
