using Application.Abstractions.Data;
using Application.Abstractions.Identity;
using Application.AuthUsers.Commands.RegisterUser;
using Application.AuthUsers.Dtos;
using Domain.AuthUsers;
using Domain.AuthUsers.Errors;
using Domain.Roles;
using FluentAssertions;
using Kernel;
using Moq;
using Xunit;

namespace Application.UnitTests.Commands;

/// <summary>
/// Unit tests for RegisterUserCommandHandler.
/// Tests cover successful registration, validation failures, and error scenarios.
/// </summary>
public class RegisterUserCommandHandlerTests
{
    private readonly Mock<ISignService<AuthUser>> _signServiceMock;
    private readonly Mock<IWriteDbContext> _dbContextMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly RegisterUserCommandHandler _handler;
    private readonly Role _defaultRole;

    public RegisterUserCommandHandlerTests()
    {
        _signServiceMock = new Mock<ISignService<AuthUser>>();
        _dbContextMock = new Mock<IWriteDbContext>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _defaultRole = Role.Create("Designer").Value;

        // Setup WriteDbContext to return the default role
        _dbContextMock.Setup(x => x.Roles)
            .Returns(TestHelpers.CreateMockDbSet(new List<Role> { _defaultRole }).Object);

        _handler = new RegisterUserCommandHandler(
            _unitOfWorkMock.Object,
            _dbContextMock.Object,
            _signServiceMock.Object);
    }

    /// <summary>
    /// Verifies that Handle successfully registers a user with valid data.
    /// This is an async method test demonstrating proper async/await handling.
    /// </summary>
    [Fact]
    public async Task Handle_WithValidRequest_ShouldRegisterUserSuccessfully()
    {
        // Arrange
        var request = new RegisterUserCommand(new RegisterRequestDto
        {
            Email = "test@example.com",
            Password = "SecurePassword123!",
            UserName = "testuser"
        });

        _signServiceMock
            .Setup(x => x.RegisterAsync(It.IsAny<AuthUser>(), It.IsAny<string>()))
            .ReturnsAsync(Result.Success());

        _unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Email.Should().Be("test@example.com");
        result.Value.UserName.Should().Be("testuser");

        _signServiceMock.Verify(
            x => x.RegisterAsync(It.IsAny<AuthUser>(), "SecurePassword123!"),
            Times.Once);

        _unitOfWorkMock.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    /// <summary>
    /// Verifies that Handle returns failure when sign service fails to register user.
    /// Tests error handling in the command handler.
    /// </summary>
    [Fact]
    public async Task Handle_WhenSignServiceFails_ShouldReturnFailure()
    {
        // Arrange
        var request = new RegisterUserCommand(new RegisterRequestDto
        {
            Email = "test@example.com",
            Password = "SecurePassword123!",
            UserName = "testuser"
        });

        var expectedError = AuthUserErrors.DuplicateEmail;
        _signServiceMock
            .Setup(x => x.RegisterAsync(It.IsAny<AuthUser>(), It.IsAny<string>()))
            .ReturnsAsync(Result.Failure(expectedError));

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(expectedError);

        _unitOfWorkMock.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }

    /// <summary>
    /// Verifies that Handle creates an AuthUser with the correct properties.
    /// </summary>
    [Fact]
    public async Task Handle_ShouldCreateAuthUserWithCorrectProperties()
    {
        // Arrange
        var request = new RegisterUserCommand(new RegisterRequestDto
        {
            Email = "newuser@example.com",
            Password = "Password123!",
            UserName = "newuser"
        });

        AuthUser? createdUser = null;
        _signServiceMock
            .Setup(x => x.RegisterAsync(It.IsAny<AuthUser>(), It.IsAny<string>()))
            .Callback<AuthUser, string>((user, _) => createdUser = user)
            .ReturnsAsync(Result.Success());

        // Act
        await _handler.Handle(request, CancellationToken.None);

        // Assert
        createdUser.Should().NotBeNull();
        createdUser!.Email.Should().Be("newuser@example.com");
        createdUser.UserName.Should().Be("newuser");
        createdUser.Roles.Should().ContainSingle();
        createdUser.Roles.First().Name.Should().Be("Designer");
    }

    /// <summary>
    /// Verifies that Handle works with different email addresses.
    /// </summary>
    [Theory]
    [InlineData("user1@example.com", "user1")]
    [InlineData("admin@company.com", "admin")]
    [InlineData("test.user@domain.org", "testuser")]
    public async Task Handle_WithDifferentEmails_ShouldRegisterSuccessfully(string email, string userName)
    {
        // Arrange
        var request = new RegisterUserCommand(new RegisterRequestDto
        {
            Email = email,
            Password = "Password123!",
            UserName = userName
        });

        _signServiceMock
            .Setup(x => x.RegisterAsync(It.IsAny<AuthUser>(), It.IsAny<string>()))
            .ReturnsAsync(Result.Success());

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Email.Should().Be(email);
        result.Value.UserName.Should().Be(userName);
    }

    /// <summary>
    /// Verifies that Handle respects cancellation tokens.
    /// </summary>
    [Fact]
    public async Task Handle_WithCancellationToken_ShouldPassToUnitOfWork()
    {
        // Arrange
        var request = new RegisterUserCommand(new RegisterRequestDto
        {
            Email = "test@example.com",
            Password = "Password123!",
            UserName = "testuser"
        });

        var cancellationToken = new CancellationToken();

        _signServiceMock
            .Setup(x => x.RegisterAsync(It.IsAny<AuthUser>(), It.IsAny<string>()))
            .ReturnsAsync(Result.Success());

        // Act
        await _handler.Handle(request, cancellationToken);

        // Assert
        _unitOfWorkMock.Verify(
            x => x.SaveChangesAsync(cancellationToken),
            Times.Once);
    }
}
