using Application.Abstractions.Data;
using Application.AuthUsers.Commands.UserRemovePermissions;
using Domain.AuthUsers;
using Domain.AuthUsers.Errors;
using Domain.Roles;
using FluentAssertions;
using Moq;
using Xunit;

namespace Application.UnitTests.Commands;

/// <summary>
/// Unit tests for UserRemovePermissionsCommandHandler.
/// Tests cover successful batch permission removal, user not found, and error scenarios.
/// </summary>
public class UserRemovePermissionsCommandHandlerTests
{
    private readonly Mock<IWriteDbContext> _dbContextMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly UserRemovePermissionsCommandHandler _handler;

    public UserRemovePermissionsCommandHandlerTests()
    {
        _dbContextMock = new Mock<IWriteDbContext>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _handler = new UserRemovePermissionsCommandHandler(_unitOfWorkMock.Object, _dbContextMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidUserAndPermissions_ShouldRemovePermissionsSuccessfully()
    {
        // Arrange
        var role = Role.Create("User").Value;
        var user = AuthUser.Create("test@example.com", "testuser", role).Value;

        var permissions = new List<PermissionDto>
        {
            new("allow", "read", "Course", "*"),
            new("allow", "update", "User", "*")
        };

        // Add permissions first
        foreach (var permDto in permissions)
        {
            var parseResult = Domain.Permissions.Permission.Parse(
                permDto.Effect, permDto.Action, permDto.Resource, permDto.ResourceId);
            user.AddPermission(parseResult.Value);
        }

        var command = new UserRemovePermissionsCommand(user.Id, permissions);

        _dbContextMock.Setup(x => x.AuthUsers)
            .Returns(TestHelpers.CreateMockDbSet(new List<AuthUser> { user }).Object);

        _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        user.Permissions.Should().BeEmpty();
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
        var command = new UserRemovePermissionsCommand(Guid.NewGuid(), permissions);

        _dbContextMock.Setup(x => x.AuthUsers)
            .Returns(TestHelpers.CreateMockDbSet(new List<AuthUser>()).Object);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(AuthUserErrors.NotFound);
    }
}
