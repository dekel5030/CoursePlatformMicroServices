using Auth.Application.Abstractions.Auth;
using Auth.Application.Abstractions.Data;
using Auth.Application.AuthUsers.Queries;
using Auth.Domain.AuthUsers;
using Auth.Domain.AuthUsers.Primitives;
using Auth.Domain.Permissions;
using Auth.Domain.Roles;
using Auth.Domain.Roles.Primitives;
using Moq;
using Xunit;
using FluentAssertions;

namespace Auth.Application.UnitTests.Queries;

public class GetMeQueryHandlerTests
{
    private readonly Mock<IReadDbContext> _dbContextMock;
    private readonly Mock<IExternalUserContext> _externalUserContextMock;
    private readonly GetMeQueryHandler _handler;

    public GetMeQueryHandlerTests()
    {
        _dbContextMock = new Mock<IReadDbContext>();
        _externalUserContextMock = new Mock<IExternalUserContext>();
        _handler = new GetMeQueryHandler(_dbContextMock.Object, _externalUserContextMock.Object);
    }

    [Fact]
    public async Task Handle_WhenUserExists_ReturnsUserDto()
    {
        // Arrange
        var identityId = "keycloak-identity-123";
        var identityProviderId = new IdentityProviderId(identityId);
        var email = new Email("test@example.com");
        var firstName = new FirstName("John");
        var lastName = new LastName("Doe");
        var fullName = new FullName(firstName, lastName);

        var roleName = new RoleName("Admin");
        var role = Role.Create(roleName).Value;
        var user = AuthUser.Create(identityProviderId, fullName, email, role).Value;

        var users = new List<AuthUser> { user };
        var mockDbSet = TestHelpers.CreateMockDbSet(users);

        _externalUserContextMock.Setup(x => x.IdentityId).Returns(identityId);
        _dbContextMock.Setup(x => x.Users).Returns(mockDbSet.Object);

        var query = new GetMeQuery();

        // Act
        var result = await _handler.Handle(query);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.Email.Should().Be("test@example.com");
        result.Value!.FirstName.Should().Be("John");
        result.Value!.LastName.Should().Be("Doe");
        result.Value!.Roles.Should().HaveCount(1);
        result.Value!.Roles.First().Name.Should().Be("admin");
    }

    [Fact]
    public async Task Handle_WhenIdentityIdIsEmpty_ReturnsFailure()
    {
        // Arrange
        _externalUserContextMock.Setup(x => x.IdentityId).Returns(string.Empty);

        var query = new GetMeQuery();

        // Act
        var result = await _handler.Handle(query);

        // Assert
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WhenUserNotFound_ReturnsFailure()
    {
        // Arrange
        var identityId = "non-existent-user";
        var users = new List<AuthUser>();
        var mockDbSet = TestHelpers.CreateMockDbSet(users);

        _externalUserContextMock.Setup(x => x.IdentityId).Returns(identityId);
        _dbContextMock.Setup(x => x.Users).Returns(mockDbSet.Object);

        var query = new GetMeQuery();

        // Act
        var result = await _handler.Handle(query);

        // Assert
        result.IsFailure.Should().BeTrue();
    }
}
