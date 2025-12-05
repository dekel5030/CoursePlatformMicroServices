using Application.Abstractions.Identity;
using Application.AuthUsers.Commands.Logout;
using Domain.AuthUsers;
using FluentAssertions;
using Moq;
using Xunit;

namespace Application.UnitTests.Commands;

/// <summary>
/// Unit tests for LogoutCommandHandler.
/// Tests cover successful logout scenarios.
/// </summary>
public class LogoutCommandHandlerTests
{
    private readonly Mock<ISignInManager<AuthUser>> _signInManagerMock;
    private readonly LogoutCommandHandler _handler;

    public LogoutCommandHandlerTests()
    {
        _signInManagerMock = new Mock<ISignInManager<AuthUser>>();
        _handler = new LogoutCommandHandler(_signInManagerMock.Object);
    }

    /// <summary>
    /// Verifies that Handle successfully logs out a user.
    /// This is an async method test demonstrating proper logout flow.
    /// </summary>
    [Fact]
    public async Task Handle_ShouldLogoutSuccessfully()
    {
        // Arrange
        var command = new LogoutCommand();

        _signInManagerMock
            .Setup(x => x.SignOutAsync())
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        _signInManagerMock.Verify(
            x => x.SignOutAsync(),
            Times.Once);
    }

    /// <summary>
    /// Verifies that Handle calls SignOutAsync on the sign-in manager.
    /// </summary>
    [Fact]
    public async Task Handle_ShouldCallSignOutAsync()
    {
        // Arrange
        var command = new LogoutCommand();

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _signInManagerMock.Verify(x => x.SignOutAsync(), Times.Once);
    }

    /// <summary>
    /// Verifies that Handle respects cancellation tokens.
    /// </summary>
    [Fact]
    public async Task Handle_WithCancellationToken_ShouldCompleteSuccessfully()
    {
        // Arrange
        var command = new LogoutCommand();
        var cancellationToken = new CancellationToken();

        _signInManagerMock
            .Setup(x => x.SignOutAsync())
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, cancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    /// <summary>
    /// Verifies that Handle can be called multiple times successfully.
    /// </summary>
    [Fact]
    public async Task Handle_CalledMultipleTimes_ShouldSucceedEachTime()
    {
        // Arrange
        var command = new LogoutCommand();

        _signInManagerMock
            .Setup(x => x.SignOutAsync())
            .Returns(Task.CompletedTask);

        // Act
        var result1 = await _handler.Handle(command, CancellationToken.None);
        var result2 = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result1.IsSuccess.Should().BeTrue();
        result2.IsSuccess.Should().BeTrue();

        _signInManagerMock.Verify(
            x => x.SignOutAsync(),
            Times.Exactly(2));
    }
}
