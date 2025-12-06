using Application.Abstractions.Identity;
using Application.AuthUsers.Commands.LoginUser;
using Application.AuthUsers.Dtos;
using Domain.AuthUsers;
using Domain.AuthUsers.Errors;
using FluentAssertions;
using Kernel;
using Moq;
using Xunit;

namespace Application.UnitTests.Commands;

/// <summary>
/// Unit tests for LoginUserCommandHandler.
/// Tests cover successful login, invalid credentials, account lockout, and error scenarios.
/// </summary>
public class LoginUserCommandHandlerTests
{
    private readonly Mock<ISignInManager<AuthUser>> _signInManagerMock;
    private readonly Mock<IUserManager<AuthUser>> _userManagerMock;
    private readonly LoginUserCommandHandler _handler;

    public LoginUserCommandHandlerTests()
    {
        _signInManagerMock = new Mock<ISignInManager<AuthUser>>();
        _userManagerMock = new Mock<IUserManager<AuthUser>>();

        _signInManagerMock.Setup(x => x.UserManager)
            .Returns(_userManagerMock.Object);

        _handler = new LoginUserCommandHandler(_signInManagerMock.Object);
    }

    /// <summary>
    /// Verifies that Handle successfully logs in a user with valid credentials.
    /// This is an async method test demonstrating proper authentication flow.
    /// </summary>
    [Fact]
    public async Task Handle_WithValidCredentials_ShouldLoginSuccessfully()
    {
        // Arrange
        var email = "test@example.com";
        var password = "SecurePassword123!";
        var user = AuthUser.Create(email, "testuser", Domain.Roles.Role.Create("User"));

        var request = new LoginUserCommand(new LoginRequestDto
        {
            Email = email,
            Password = password
        });

        _userManagerMock
            .Setup(x => x.FindByEmailAsync(email))
            .ReturnsAsync(user);

        _signInManagerMock
            .Setup(x => x.PasswordSignInAsync(user, password, true, true))
            .ReturnsAsync(Result.Success());

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Email.Should().Be(email);
        result.Value.UserName.Should().Be("testuser");
        result.Value.Id.Should().Be(user.Id);

        _userManagerMock.Verify(x => x.FindByEmailAsync(email), Times.Once);
        _signInManagerMock.Verify(
            x => x.PasswordSignInAsync(user, password, true, true),
            Times.Once);
    }

    /// <summary>
    /// Verifies that Handle returns failure when user is not found.
    /// Tests error handling for non-existent users.
    /// </summary>
    [Fact]
    public async Task Handle_WhenUserNotFound_ShouldReturnInvalidCredentialsError()
    {
        // Arrange
        var request = new LoginUserCommand(new LoginRequestDto
        {
            Email = "nonexistent@example.com",
            Password = "Password123!"
        });

        _userManagerMock
            .Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((AuthUser?)null);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(AuthUserErrors.InvalidCredentials);

        _signInManagerMock.Verify(
            x => x.PasswordSignInAsync(It.IsAny<AuthUser>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()),
            Times.Never);
    }

    /// <summary>
    /// Verifies that Handle returns failure when password is incorrect.
    /// Tests password validation error handling.
    /// </summary>
    [Fact]
    public async Task Handle_WithInvalidPassword_ShouldReturnFailure()
    {
        // Arrange
        var email = "test@example.com";
        var user = AuthUser.Create(email, "testuser", Domain.Roles.Role.Create("User"));

        var request = new LoginUserCommand(new LoginRequestDto
        {
            Email = email,
            Password = "WrongPassword!"
        });

        var expectedError = AuthUserErrors.InvalidCredentials;

        _userManagerMock
            .Setup(x => x.FindByEmailAsync(email))
            .ReturnsAsync(user);

        _signInManagerMock
            .Setup(x => x.PasswordSignInAsync(It.IsAny<AuthUser>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync(Result.Failure(expectedError));

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(expectedError);
    }

    /// <summary>
    /// Verifies that Handle returns failure when account is locked out.
    /// Tests account lockout scenario.
    /// </summary>
    [Fact]
    public async Task Handle_WhenAccountLockedOut_ShouldReturnLockoutError()
    {
        // Arrange
        var email = "locked@example.com";
        var user = AuthUser.Create(email, "lockeduser", Domain.Roles.Role.Create("User"));

        var request = new LoginUserCommand(new LoginRequestDto
        {
            Email = email,
            Password = "Password123!"
        });

        var expectedError = AuthUserErrors.IsLockOut;

        _userManagerMock
            .Setup(x => x.FindByEmailAsync(email))
            .ReturnsAsync(user);

        _signInManagerMock
            .Setup(x => x.PasswordSignInAsync(It.IsAny<AuthUser>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync(Result.Failure(expectedError));

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(expectedError);
    }

    /// <summary>
    /// Verifies that Handle works with different email addresses.
    /// </summary>
    [Theory]
    [InlineData("user1@example.com", "user1")]
    [InlineData("admin@company.com", "admin")]
    [InlineData("test.user@domain.org", "testuser")]
    public async Task Handle_WithDifferentEmails_ShouldLoginSuccessfully(string email, string userName)
    {
        // Arrange
        var user = AuthUser.Create(email, userName, Domain.Roles.Role.Create("User"));

        var request = new LoginUserCommand(new LoginRequestDto
        {
            Email = email,
            Password = "Password123!"
        });

        _userManagerMock
            .Setup(x => x.FindByEmailAsync(email))
            .ReturnsAsync(user);

        _signInManagerMock
            .Setup(x => x.PasswordSignInAsync(It.IsAny<AuthUser>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync(Result.Success());

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Email.Should().Be(email);
        result.Value.UserName.Should().Be(userName);
    }

    /// <summary>
    /// Verifies that Handle sets the persistent and lockout flags correctly.
    /// </summary>
    [Fact]
    public async Task Handle_ShouldSetPersistentAndLockoutFlags()
    {
        // Arrange
        var email = "test@example.com";
        var password = "Password123!";
        var user = AuthUser.Create(email, "testuser", Domain.Roles.Role.Create("User"));

        var request = new LoginUserCommand(new LoginRequestDto
        {
            Email = email,
            Password = password
        });

        _userManagerMock
            .Setup(x => x.FindByEmailAsync(email))
            .ReturnsAsync(user);

        _signInManagerMock
            .Setup(x => x.PasswordSignInAsync(user, password, true, true))
            .ReturnsAsync(Result.Success());

        // Act
        await _handler.Handle(request, CancellationToken.None);

        // Assert
        _signInManagerMock.Verify(
            x => x.PasswordSignInAsync(user, password, true, true),
            Times.Once);
    }
}
