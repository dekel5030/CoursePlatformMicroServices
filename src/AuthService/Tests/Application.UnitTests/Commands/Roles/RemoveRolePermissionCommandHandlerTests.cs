using Application.Abstractions.Data;
using Application.Roles.Commands.RemoveRolePermission;
using Domain.Permissions;
using Domain.Roles;
using Domain.Roles.Errors;
using FluentAssertions;
using Kernel;
using Kernel.Auth.AuthTypes;
using Moq;
using Xunit;

namespace Application.UnitTests.Commands.Roles;

/// <summary>
/// Unit tests for RemoveRolePermissionCommandHandler.
/// Tests cover successful permission removal from role, role not found, invalid permission, and error scenarios.
/// </summary>
public class RemoveRolePermissionCommandHandlerTests
{
    private readonly Mock<IWriteDbContext> _dbContextMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly RoleRemovePermissionCommandHandler _handler;

    public RemoveRolePermissionCommandHandlerTests()
    {
        _dbContextMock = new Mock<IWriteDbContext>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _handler = new RoleRemovePermissionCommandHandler(_dbContextMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidRoleAndPermission_ShouldRemovePermissionSuccessfully()
    {
        var role = Role.Create("Admin").Value;
        // Create permission using constructor with proper enum types - must match the command exactly
        var permission = new Permission(EffectType.Allow, ActionType.Read, ResourceType.Course, ResourceId.Wildcard);
        role.AddPermission(permission);
        var command = new RoleRemovePermissionCommand(role.Id, "allow", "read", "Course", "*");

        _dbContextMock.Setup(x => x.Roles)
            .Returns(TestHelpers.CreateMockDbSet(new List<Role> { role }).Object);
        _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WhenRoleNotFound_ShouldReturnNotFoundError()
    {
        var command = new RoleRemovePermissionCommand(Guid.NewGuid(), "allow", "read", "Course", "*");

        _dbContextMock.Setup(x => x.Roles)
            .Returns(TestHelpers.CreateMockDbSet(new List<Role>()).Object);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(RoleErrors.NotFound);
    }

    [Fact]
    public async Task Handle_WithInvalidPermission_ShouldReturnFailure()
    {
        var command = new RoleRemovePermissionCommand(Guid.NewGuid(), "invalid_effect", "read", "Course", "*");

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
    }
}
