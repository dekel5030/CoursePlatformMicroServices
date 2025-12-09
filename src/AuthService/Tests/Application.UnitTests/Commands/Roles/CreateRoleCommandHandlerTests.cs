using Application.Abstractions.Data;
using Application.Roles.Commands.CreateRole;
using Domain.Roles;
using FluentAssertions;
using Kernel;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace Application.UnitTests.Commands.Roles;

/// <summary>
/// Unit tests for CreateRoleCommandHandler.
/// Tests cover successful role creation, validation failures, and error scenarios.
/// </summary>
public class CreateRoleCommandHandlerTests
{
    private readonly Mock<IWriteDbContext> _dbContextMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<DbSet<Role>> _rolesDbSetMock;
    private readonly CreateRoleCommandHandler _handler;

    public CreateRoleCommandHandlerTests()
    {
        _dbContextMock = new Mock<IWriteDbContext>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _rolesDbSetMock = new Mock<DbSet<Role>>();

        _dbContextMock.Setup(x => x.Roles).Returns(_rolesDbSetMock.Object);

        _handler = new CreateRoleCommandHandler(_dbContextMock.Object, _unitOfWorkMock.Object);
    }

    /// <summary>
    /// Verifies that Handle successfully creates a role with valid data.
    /// </summary>
    [Fact]
    public async Task Handle_WithValidRoleName_ShouldCreateRoleSuccessfully()
    {
        // Arrange
        var roleName = "Editor";
        var command = new CreateRoleCommand(roleName);

        _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.RoleName.Should().Be(roleName);

        _rolesDbSetMock.Verify(
            x => x.AddAsync(It.IsAny<Role>(), It.IsAny<CancellationToken>()),
            Times.Once);

        _unitOfWorkMock.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    /// <summary>
    /// Verifies that Handle returns failure when role creation fails.
    /// </summary>
    [Fact]
    public async Task Handle_WhenRoleCreationFails_ShouldReturnFailure()
    {
        // Arrange
        var roleName = ""; // Empty name should fail validation
        var command = new CreateRoleCommand(roleName);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();

        _rolesDbSetMock.Verify(
            x => x.AddAsync(It.IsAny<Role>(), It.IsAny<CancellationToken>()),
            Times.Never);

        _unitOfWorkMock.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }

    /// <summary>
    /// Verifies that Handle creates role with correct properties.
    /// </summary>
    [Theory]
    [InlineData("Admin")]
    [InlineData("User")]
    [InlineData("Editor")]
    [InlineData("Moderator")]
    public async Task Handle_WithDifferentRoleNames_ShouldCreateSuccessfully(string roleName)
    {
        // Arrange
        var command = new CreateRoleCommand(roleName);

        _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.RoleName.Should().Be(roleName);
    }

    /// <summary>
    /// Verifies that Handle respects cancellation tokens.
    /// </summary>
    [Fact]
    public async Task Handle_WithCancellationToken_ShouldPassToUnitOfWork()
    {
        // Arrange
        var roleName = "Editor";
        var command = new CreateRoleCommand(roleName);
        var cancellationToken = new CancellationToken();

        _unitOfWorkMock.Setup(x => x.SaveChangesAsync(cancellationToken))
            .ReturnsAsync(1);

        // Act
        await _handler.Handle(command, cancellationToken);

        // Assert
        _unitOfWorkMock.Verify(
            x => x.SaveChangesAsync(cancellationToken),
            Times.Once);
    }

    /// <summary>
    /// Verifies that Handle returns DTO with role ID and name.
    /// </summary>
    [Fact]
    public async Task Handle_ShouldReturnDtoWithRoleIdAndName()
    {
        // Arrange
        var roleName = "TestRole";
        var command = new CreateRoleCommand(roleName);

        _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Value.Should().NotBeNull();
        result.Value.RoleId.Should().NotBeNullOrEmpty();
        result.Value.RoleName.Should().Be(roleName);
    }
}
