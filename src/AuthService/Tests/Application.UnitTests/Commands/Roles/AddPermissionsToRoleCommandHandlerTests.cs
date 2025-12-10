using Application.Abstractions.Data;
using Application.Roles.Commands.RoleAddPermissions;
using Domain.Roles;
using Domain.Roles.Errors;
using FluentAssertions;
using Moq;
using Xunit;

namespace Application.UnitTests.Commands.Roles;

/// <summary>
/// Unit tests for AddPermissionsToRoleCommandHandler.
/// Tests cover successful batch permission addition to role, role not found, invalid permissions, and error scenarios.
/// </summary>
public class AddPermissionsToRoleCommandHandlerTests
{
    private readonly Mock<IWriteDbContext> _dbContextMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly RoleAddPermissionsCommandHandler _handler;

    public AddPermissionsToRoleCommandHandlerTests()
    {
        _dbContextMock = new Mock<IWriteDbContext>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _handler = new RoleAddPermissionsCommandHandler(_unitOfWorkMock.Object, _dbContextMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidRoleAndPermissions_ShouldAddPermissionsSuccessfully()
    {
        // Arrange
        var role = Role.Create("Admin").Value;
        var permissions = new List<PermissionDto>
        {
            new("allow", "read", "Course", "*"),
            new("allow", "update", "User", "*"),
            new("allow", "delete", "Lesson", "*")
        };
        var command = new RoleAddPermissionsCommand(role.Id, permissions);

        _dbContextMock.Setup(x => x.Roles.FindAsync(role.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(role);
        _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        role.Permissions.Should().HaveCount(3);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenRoleNotFound_ShouldReturnNotFoundError()
    {
        // Arrange
        var permissions = new List<PermissionDto>
        {
            new("allow", "read", "Course", "*")
        };
        var command = new RoleAddPermissionsCommand(Guid.NewGuid(), permissions);

        _dbContextMock.Setup(x => x.Roles.FindAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Role?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(RoleErrors.NotFound);
    }

    [Fact]
    public async Task Handle_WithInvalidPermission_ShouldReturnFailure()
    {
        // Arrange
        var role = Role.Create("Admin").Value;
        var permissions = new List<PermissionDto>
        {
            new("invalid_effect", "read", "Course", "*")
        };
        var command = new RoleAddPermissionsCommand(role.Id, permissions);

        _dbContextMock.Setup(x => x.Roles.FindAsync(role.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(role);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WithDenyEffect_ShouldReturnFailure()
    {
        // Arrange
        var role = Role.Create("Admin").Value;
        var permissions = new List<PermissionDto>
        {
            new("deny", "read", "Course", "*")
        };
        var command = new RoleAddPermissionsCommand(role.Id, permissions);

        _dbContextMock.Setup(x => x.Roles.FindAsync(role.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(role);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Role.InvalidPermissionEffect");
    }
}
