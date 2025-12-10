using Application.Abstractions.Data;
using Application.AuthUsers.Commands.UserAddPermissions;
using Domain.AuthUsers;
using Domain.AuthUsers.Errors;
using Domain.Roles;
using FluentAssertions;
using Moq;
using Xunit;

namespace Application.UnitTests.Commands;

/// <summary>
/// Unit tests for UserAddPermissionsCommandHandler.
/// Tests cover successful batch permission addition, user not found, invalid permissions, and error scenarios.
/// </summary>
public class UserAddPermissionsCommandHandlerTests
{
    private readonly Mock<IWriteDbContext> _dbContextMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly UserAddPermissionsCommandHandler _handler;

    public UserAddPermissionsCommandHandlerTests()
    {
        _dbContextMock = new Mock<IWriteDbContext>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _handler = new UserAddPermissionsCommandHandler(_unitOfWorkMock.Object, _dbContextMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidUserAndPermissions_ShouldAddPermissionsSuccessfully()
    {
        // Arrange
        var role = Role.Create("User").Value;
        var user = AuthUser.Create("test@example.com", "testuser", role).Value;

        var permissions = new List<PermissionDto>
        {
            new("allow", "read", "Course", "*"),
            new("allow", "update", "User", "*"),
            new("deny", "delete", "Lesson", "*")
        };
        var command = new UserAddPermissionsCommand(user.Id, permissions);

        _dbContextMock.Setup(x => x.AuthUsers)
            .Returns(TestHelpers.CreateMockDbSet(new List<AuthUser> { user }).Object);

        _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        user.Permissions.Should().HaveCount(3);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenUserNotFound_ShouldReturnNotFoundError()
    {
        // Arrange
        var permissions = new List<PermissionDto>
        {
            new("allow", "read", "Course", "*")
        };
        var command = new UserAddPermissionsCommand(Guid.NewGuid(), permissions);

        _dbContextMock.Setup(x => x.AuthUsers)
            .Returns(TestHelpers.CreateMockDbSet(new List<AuthUser>()).Object);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(AuthUserErrors.NotFound);
    }

    [Fact]
    public async Task Handle_WithInvalidPermission_ShouldReturnFailure()
    {
        // Arrange
        var role = Role.Create("User").Value;
        var user = AuthUser.Create("test@example.com", "testuser", role).Value;

        var permissions = new List<PermissionDto>
        {
            new("invalid_effect", "read", "Course", "*")
        };
        var command = new UserAddPermissionsCommand(user.Id, permissions);

        _dbContextMock.Setup(x => x.AuthUsers)
            .Returns(TestHelpers.CreateMockDbSet(new List<AuthUser> { user }).Object);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
