using Application.Abstractions.Data;
using Application.Roles.Commands.AddRolePermission;
using Domain.Permissions;
using Domain.Roles;
using Domain.Roles.Errors;
using FluentAssertions;
using Kernel;
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
    private readonly AddRolePermissionCommandHandler _handler;

    public AddRolePermissionCommandHandlerTests()
    {
        _dbContextMock = new Mock<IWriteDbContext>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _handler = new AddRolePermissionCommandHandler(_unitOfWorkMock.Object, _dbContextMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidRoleAndPermission_ShouldAddPermissionSuccessfully()
    {
        var role = Role.Create("Admin").Value;
        var command = new AddRolePermissionCommand(role.Id, "allow", "read", "posts", "*");

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
        var command = new AddRolePermissionCommand(Guid.NewGuid(), "allow", "read", "posts", "*");

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
        var command = new AddRolePermissionCommand(role.Id, "invalid_effect", "read", "posts", "*");

        _dbContextMock.Setup(x => x.Roles.FindAsync(role.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(role);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
    }
}
