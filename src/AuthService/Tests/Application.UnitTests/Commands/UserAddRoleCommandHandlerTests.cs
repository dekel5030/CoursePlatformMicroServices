using Application.Abstractions.Data;
using Application.AuthUsers.Commands.UserAddRole;
using Domain.AuthUsers;
using Domain.AuthUsers.Errors;
using Domain.Roles;
using Domain.Roles.Errors;
using FluentAssertions;
using Moq;
using Xunit;

namespace Application.UnitTests.Commands;

public class UserAddRoleCommandHandlerTests
{
    private readonly Mock<IWriteDbContext> _dbContextMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly UserAddRoleCommandHandler _handler;

    public UserAddRoleCommandHandlerTests()
    {
        _dbContextMock = new Mock<IWriteDbContext>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _handler = new UserAddRoleCommandHandler(_dbContextMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidUserAndRole_ShouldAddRoleSuccessfully()
    {
        var role = Role.Create("Admin").Value;
        var user = AuthUser.Create("test@example.com", "testuser", role).Value;
        var newRole = Role.Create("Editor").Value;
        var command = new UserAddRoleCommand(user.Id, "Editor");

        _dbContextMock.Setup(x => x.AuthUsers)
            .Returns(TestHelpers.CreateMockDbSet(new List<AuthUser> { user }).Object);
        _dbContextMock.Setup(x => x.Roles)
            .Returns(TestHelpers.CreateMockDbSet(new List<Role> { role, newRole }).Object);
        _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WhenUserNotFound_ShouldReturnNotFoundError()
    {
        var command = new UserAddRoleCommand(Guid.NewGuid(), "Admin");
        _dbContextMock.Setup(x => x.AuthUsers)
            .Returns(TestHelpers.CreateMockDbSet(new List<AuthUser>()).Object);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(AuthUserErrors.NotFound);
    }

    [Fact]
    public async Task Handle_WhenRoleNotFound_ShouldReturnNotFoundError()
    {
        var role = Role.Create("User").Value;
        var user = AuthUser.Create("test@example.com", "testuser", role).Value;
        var command = new UserAddRoleCommand(user.Id, "NonExistentRole");

        _dbContextMock.Setup(x => x.AuthUsers)
            .Returns(TestHelpers.CreateMockDbSet(new List<AuthUser> { user }).Object);
        _dbContextMock.Setup(x => x.Roles)
            .Returns(TestHelpers.CreateMockDbSet(new List<Role> { role }).Object);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(RoleErrors.NotFound);
    }
}
