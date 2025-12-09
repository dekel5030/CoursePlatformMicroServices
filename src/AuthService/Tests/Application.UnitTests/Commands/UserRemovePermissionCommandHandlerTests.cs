using Application.Abstractions.Data;
using Application.AuthUsers.Commands.UserRemovePermission;
using Domain.AuthUsers;
using Domain.AuthUsers.Errors;
using Domain.Permissions;
using Domain.Roles;
using FluentAssertions;
using Kernel;
using Moq;
using Xunit;

namespace Application.UnitTests.Commands;

/// <summary>
/// Unit tests for UserRemovePermissionCommandHandler.
/// Tests cover successful permission removal, user not found, invalid permission, and error scenarios.
/// </summary>
public class UserRemovePermissionCommandHandlerTests
{
    private readonly Mock<IWriteDbContext> _dbContextMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly UserRemovePermissionCommandHandler _handler;

    public UserRemovePermissionCommandHandlerTests()
    {
        _dbContextMock = new Mock<IWriteDbContext>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _handler = new UserRemovePermissionCommandHandler(_unitOfWorkMock.Object, _dbContextMock.Object);
    }

    /// <summary>
    /// Verifies that Handle successfully removes a permission from a user.
    /// </summary>
    [Fact]
    public async Task Handle_WithValidUserAndPermission_ShouldRemovePermissionSuccessfully()
    {
        // Arrange
        var role = Role.Create("User").Value;
        var user = AuthUser.Create("test@example.com", "testuser", role).Value;
        
        var permission = Permission.Parse("allow", "read", "posts", "*").Value;
        user.AddPermission(permission);

        var command = new UserRemovePermissionCommand(
            UserId: user.Id,
            Effect: "allow",
            Action: "read",
            Resource: "posts",
            ResourceId: "*"
        );

        _dbContextMock.Setup(x => x.AuthUsers)
            .Returns(TestHelpers.CreateMockDbSet(new List<AuthUser> { user }).Object);

        _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        
        _unitOfWorkMock.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    /// <summary>
    /// Verifies that Handle returns NotFound when user doesn't exist.
    /// </summary>
    [Fact]
    public async Task Handle_WhenUserNotFound_ShouldReturnNotFoundError()
    {
        // Arrange
        var command = new UserRemovePermissionCommand(
            UserId: Guid.NewGuid(),
            Effect: "allow",
            Action: "read",
            Resource: "posts",
            ResourceId: "*"
        );

        _dbContextMock.Setup(x => x.AuthUsers)
            .Returns(TestHelpers.CreateMockDbSet(new List<AuthUser>()).Object);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(AuthUserErrors.NotFound);

        _unitOfWorkMock.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }

    /// <summary>
    /// Verifies that Handle returns failure when permission parsing fails.
    /// </summary>
    [Fact]
    public async Task Handle_WithInvalidPermission_ShouldReturnFailure()
    {
        // Arrange
        var role = Role.Create("User").Value;
        var user = AuthUser.Create("test@example.com", "testuser", role).Value;

        var command = new UserRemovePermissionCommand(
            UserId: user.Id,
            Effect: "invalid_effect",
            Action: "read",
            Resource: "posts",
            ResourceId: "*"
        );

        _dbContextMock.Setup(x => x.AuthUsers)
            .Returns(TestHelpers.CreateMockDbSet(new List<AuthUser> { user }).Object);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();

        _unitOfWorkMock.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }

    /// <summary>
    /// Verifies that Handle returns failure when removing permission user doesn't have.
    /// </summary>
    [Fact]
    public async Task Handle_WhenPermissionDoesNotExist_ShouldReturnFailure()
    {
        // Arrange
        var role = Role.Create("User").Value;
        var user = AuthUser.Create("test@example.com", "testuser", role).Value;

        var command = new UserRemovePermissionCommand(
            UserId: user.Id,
            Effect: "allow",
            Action: "read",
            Resource: "posts",
            ResourceId: "*"
        );

        _dbContextMock.Setup(x => x.AuthUsers)
            .Returns(TestHelpers.CreateMockDbSet(new List<AuthUser> { user }).Object);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();

        _unitOfWorkMock.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }

    /// <summary>
    /// Verifies that Handle works with different permission types.
    /// </summary>
    [Theory]
    [InlineData("allow", "read", "posts", "*")]
    [InlineData("deny", "write", "comments", "123")]
    [InlineData("allow", "delete", "users", "456")]
    public async Task Handle_WithDifferentPermissions_ShouldRemoveSuccessfully(
        string effect, string action, string resource, string resourceId)
    {
        // Arrange
        var role = Role.Create("User").Value;
        var user = AuthUser.Create("test@example.com", "testuser", role).Value;
        
        var permission = Permission.Parse(effect, action, resource, resourceId).Value;
        user.AddPermission(permission);

        var command = new UserRemovePermissionCommand(
            UserId: user.Id,
            Effect: effect,
            Action: action,
            Resource: resource,
            ResourceId: resourceId
        );

        _dbContextMock.Setup(x => x.AuthUsers)
            .Returns(TestHelpers.CreateMockDbSet(new List<AuthUser> { user }).Object);

        _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    /// <summary>
    /// Verifies that Handle respects cancellation tokens.
    /// </summary>
    [Fact]
    public async Task Handle_WithCancellationToken_ShouldPassToUnitOfWork()
    {
        // Arrange
        var role = Role.Create("User").Value;
        var user = AuthUser.Create("test@example.com", "testuser", role).Value;
        
        var permission = Permission.Parse("allow", "read", "posts", "*").Value;
        user.AddPermission(permission);
        
        var cancellationToken = new CancellationToken();

        var command = new UserRemovePermissionCommand(
            UserId: user.Id,
            Effect: "allow",
            Action: "read",
            Resource: "posts",
            ResourceId: "*"
        );

        _dbContextMock.Setup(x => x.AuthUsers)
            .Returns(TestHelpers.CreateMockDbSet(new List<AuthUser> { user }).Object);

        // Act
        await _handler.Handle(command, cancellationToken);

        // Assert
        _unitOfWorkMock.Verify(
            x => x.SaveChangesAsync(cancellationToken),
            Times.Once);
    }
}
