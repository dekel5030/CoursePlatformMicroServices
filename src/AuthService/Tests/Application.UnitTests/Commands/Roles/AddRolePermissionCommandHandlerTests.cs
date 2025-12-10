using Application.Abstractions.Data;
using Application.Roles.Commands.AddRolePermission;
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
/// Unit tests for AddRolePermissionCommandHandler.
/// Tests cover successful permission addition to role, role not found, invalid permission, and error scenarios.
/// </summary>
public class AddRolePermissionCommandHandlerTests
{
    private readonly Mock<IWriteDbContext> _dbContextMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly RoleAddPermissionCommandHandler _handler;

    public AddRolePermissionCommandHandlerTests()
    {
        _dbContextMock = new Mock<IWriteDbContext>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _handler = new RoleAddPermissionCommandHandler(_unitOfWorkMock.Object, _dbContextMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidRoleAndPermission_ShouldAddPermissionSuccessfully()
    {
        var role = Role.Create("Admin").Value;
        var command = new RoleAddPermissionCommand(role.Id, "allow", "read", "Course", "*");

        _dbContextMock.Setup(x => x.Roles.FindAsync(role.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(role);
        _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenRoleNotFound_ShouldReturnNotFoundError()
    {
        var command = new RoleAddPermissionCommand(Guid.NewGuid(), "allow", "read", "Course", "*");

        _dbContextMock.Setup(x => x.Roles.FindAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Role?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(RoleErrors.NotFound);
    }

    [Fact]
    public async Task Handle_WithInvalidPermission_ShouldReturnFailure()
    {
        var role = Role.Create("Admin").Value;
        var command = new RoleAddPermissionCommand(role.Id, "invalid_effect", "read", "Course", "*");

        _dbContextMock.Setup(x => x.Roles.FindAsync(role.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(role);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
    }
}
