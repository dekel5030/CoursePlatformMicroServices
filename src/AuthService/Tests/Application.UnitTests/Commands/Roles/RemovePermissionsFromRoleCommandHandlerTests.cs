using Application.Abstractions.Data;
using Application.Roles.Commands.RemovePermissionsFromRole;
using Domain.Roles;
using Domain.Roles.Errors;
using FluentAssertions;
using Moq;
using Xunit;

namespace Application.UnitTests.Commands.Roles;

/// <summary>
/// Unit tests for RemovePermissionsFromRoleCommandHandler.
/// Tests cover successful batch permission removal from role, role not found, and error scenarios.
/// </summary>
public class RemovePermissionsFromRoleCommandHandlerTests
{
    private readonly Mock<IWriteDbContext> _dbContextMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly RoleRemovePermissionsCommandHandler _handler;

    public RemovePermissionsFromRoleCommandHandlerTests()
    {
        _dbContextMock = new Mock<IWriteDbContext>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _handler = new RoleRemovePermissionsCommandHandler(_unitOfWorkMock.Object, _dbContextMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidRoleAndPermissions_ShouldRemovePermissionsSuccessfully()
    {
        // Arrange
        var role = Role.Create("Admin").Value;
        var permissions = new List<PermissionDto>
        {
            new("allow", "read", "Course", "*"),
            new("allow", "update", "User", "*")
        };

        // Add permissions first
        foreach (var permDto in permissions)
        {
            var parseResult = Domain.Permissions.Permission.Parse(
                permDto.Effect, permDto.Action, permDto.Resource, permDto.ResourceId ?? "*");
            role.AddPermission(parseResult.Value);
        }

        var command = new RoleRemovePermissionsCommand(role.Id, permissions);

        _dbContextMock.Setup(x => x.Roles.FindAsync(role.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(role);
        _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        role.Permissions.Should().BeEmpty();
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
        var command = new RoleRemovePermissionsCommand(Guid.NewGuid(), permissions);

        _dbContextMock.Setup(x => x.Roles.FindAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Role?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(RoleErrors.NotFound);
    }
}
