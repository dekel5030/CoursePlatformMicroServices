using System.Security.Claims;
using System.Text.Encodings.Web;
using CoursePlatform.ServiceDefaults.Auth;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace ServiceDefaults.UnitTests.Auth;

public class GatewayHeaderAuthenticationHandlerTests
{
    private readonly Mock<IOptionsMonitor<AuthenticationSchemeOptions>> _optionsMock;
    private readonly Mock<ILoggerFactory> _loggerFactoryMock;
    private readonly Mock<ILogger<GatewayHeaderAuthenticationHandler>> _loggerMock;
    private readonly UrlEncoder _encoder;
    private readonly AuthenticationScheme _scheme;
    private readonly DefaultHttpContext _context;

    public GatewayHeaderAuthenticationHandlerTests()
    {
        _optionsMock = new Mock<IOptionsMonitor<AuthenticationSchemeOptions>>();
        _loggerFactoryMock = new Mock<ILoggerFactory>();
        _loggerMock = new Mock<ILogger<GatewayHeaderAuthenticationHandler>>();
        _encoder = UrlEncoder.Default;
        _scheme = new AuthenticationScheme(
            GatewayHeaderAuthenticationHandler.SchemeName,
            null,
            typeof(GatewayHeaderAuthenticationHandler));
        _context = new DefaultHttpContext();

        _optionsMock
            .Setup(x => x.Get(It.IsAny<string>()))
            .Returns(new AuthenticationSchemeOptions());

        _loggerFactoryMock
            .Setup(x => x.CreateLogger(It.IsAny<string>()))
            .Returns(_loggerMock.Object);
    }

    private GatewayHeaderAuthenticationHandler CreateHandler()
    {
        var handler = new GatewayHeaderAuthenticationHandler(
            _optionsMock.Object,
            _loggerFactoryMock.Object,
            _encoder);

        handler.InitializeAsync(_scheme, _context).GetAwaiter().GetResult();
        return handler;
    }

    [Fact]
    public async Task HandleAuthenticateAsync_ShouldReturnNoResult_WhenUserIdHeaderIsMissingAsync()
    {
        // Arrange
        var handler = CreateHandler();

        // Act
        var result = await handler.AuthenticateAsync();

        // Assert
        result.Succeeded.Should().BeFalse();
        result.None.Should().BeTrue();
    }

    [Fact]
    public async Task HandleAuthenticateAsync_ShouldReturnFailure_WhenUserIdHeaderIsEmptyAsync()
    {
        // Arrange
        _context.Request.Headers[HeaderNames.UserIdHeader] = string.Empty;
        var handler = CreateHandler();

        // Act
        var result = await handler.AuthenticateAsync();

        // Assert
        result.Succeeded.Should().BeFalse();
        result.Failure.Should().NotBeNull();
        result.Failure!.Message.Should().Be("User ID header is empty.");
    }

    [Fact]
    public async Task HandleAuthenticateAsync_ShouldSucceed_WithValidUserIdAsync()
    {
        // Arrange
        _context.Request.Headers[HeaderNames.UserIdHeader] = "user123";
        var handler = CreateHandler();

        // Act
        var result = await handler.AuthenticateAsync();

        // Assert
        result.Succeeded.Should().BeTrue();
        result.Principal.Should().NotBeNull();
        result.Principal!.Identity!.IsAuthenticated.Should().BeTrue();
        result.Principal.FindFirst(ClaimTypes.NameIdentifier)!.Value.Should().Be("user123");
        result.Principal.FindFirst("sub")!.Value.Should().Be("user123");
    }

    [Fact]
    public async Task HandleAuthenticateAsync_ShouldAddSingleRole_WhenSingleRoleHeaderProvidedAsync()
    {
        // Arrange
        _context.Request.Headers[HeaderNames.UserIdHeader] = "user123";
        _context.Request.Headers[HeaderNames.UserRoleHeader] = "Admin";
        var handler = CreateHandler();

        // Act
        var result = await handler.AuthenticateAsync();

        // Assert
        result.Succeeded.Should().BeTrue();
        var roleClaims = result.Principal!.FindAll(ClaimTypes.Role).ToList();
        roleClaims.Should().HaveCount(1);
        roleClaims[0].Value.Should().Be("Admin");
    }

    [Fact]
    public async Task HandleAuthenticateAsync_ShouldAddMultipleRoles_WhenMultipleRoleHeadersProvidedAsync()
    {
        // Arrange
        _context.Request.Headers[HeaderNames.UserIdHeader] = "user123";
        _context.Request.Headers.Append(HeaderNames.UserRoleHeader, "Admin");
        _context.Request.Headers.Append(HeaderNames.UserRoleHeader, "User");
        _context.Request.Headers.Append(HeaderNames.UserRoleHeader, "Instructor");
        var handler = CreateHandler();

        // Act
        var result = await handler.AuthenticateAsync();

        // Assert
        result.Succeeded.Should().BeTrue();
        var roleClaims = result.Principal!.FindAll(ClaimTypes.Role).ToList();
        roleClaims.Should().HaveCount(3);
        roleClaims.Select(c => c.Value).Should().Contain(new[] { "Admin", "User", "Instructor" });
    }

    [Fact]
    public async Task HandleAuthenticateAsync_ShouldIgnoreEmptyRolesAsync()
    {
        // Arrange
        _context.Request.Headers[HeaderNames.UserIdHeader] = "user123";
        _context.Request.Headers.Append(HeaderNames.UserRoleHeader, "Admin");
        _context.Request.Headers.Append(HeaderNames.UserRoleHeader, "");
        _context.Request.Headers.Append(HeaderNames.UserRoleHeader, "User");
        var handler = CreateHandler();

        // Act
        var result = await handler.AuthenticateAsync();

        // Assert
        result.Succeeded.Should().BeTrue();
        var roleClaims = result.Principal!.FindAll(ClaimTypes.Role).ToList();
        roleClaims.Should().HaveCount(2);
        roleClaims.Select(c => c.Value).Should().Contain(new[] { "Admin", "User" });
    }

    [Fact]
    public async Task HandleAuthenticateAsync_ShouldAddValidPermissionsAsync()
    {
        // Arrange
        _context.Request.Headers[HeaderNames.UserIdHeader] = "user123";
        _context.Request.Headers.Append(HeaderNames.UserPermissionsHeader, "allow:read:course:123");
        _context.Request.Headers.Append(HeaderNames.UserPermissionsHeader, "allow:update:course:456");
        var handler = CreateHandler();

        // Act
        var result = await handler.AuthenticateAsync();

        // Assert
        result.Succeeded.Should().BeTrue();
        var permissionClaims = result.Principal!.FindAll(PermissionClaim.ClaimType).ToList();
        permissionClaims.Should().HaveCount(2);
        permissionClaims.Select(c => c.Value).Should().Contain(new[]
        {
            "allow:read:course:123",
            "allow:update:course:456"
        });
    }

    [Fact]
    public async Task HandleAuthenticateAsync_ShouldIgnoreInvalidPermissionsAsync()
    {
        // Arrange
        _context.Request.Headers[HeaderNames.UserIdHeader] = "user123";
        _context.Request.Headers.Append(HeaderNames.UserPermissionsHeader, "allow:read:course:123");
        _context.Request.Headers.Append(HeaderNames.UserPermissionsHeader, "invalid-permission");
        _context.Request.Headers.Append(HeaderNames.UserPermissionsHeader, "deny:delete:user:789");
        var handler = CreateHandler();

        // Act
        var result = await handler.AuthenticateAsync();

        // Assert
        result.Succeeded.Should().BeTrue();
        var permissionClaims = result.Principal!.FindAll(PermissionClaim.ClaimType).ToList();
        permissionClaims.Should().HaveCount(2);
        permissionClaims.Select(c => c.Value).Should().Contain(new[]
        {
            "allow:read:course:123",
            "deny:delete:user:789"
        });
    }

    [Fact]
    public async Task HandleAuthenticateAsync_ShouldIgnoreEmptyPermissionsAsync()
    {
        // Arrange
        _context.Request.Headers[HeaderNames.UserIdHeader] = "user123";
        _context.Request.Headers.Append(HeaderNames.UserPermissionsHeader, "allow:read:course:123");
        _context.Request.Headers.Append(HeaderNames.UserPermissionsHeader, "");
        _context.Request.Headers.Append(HeaderNames.UserPermissionsHeader, "deny:delete:user:789");
        var handler = CreateHandler();

        // Act
        var result = await handler.AuthenticateAsync();

        // Assert
        result.Succeeded.Should().BeTrue();
        var permissionClaims = result.Principal!.FindAll(PermissionClaim.ClaimType).ToList();
        permissionClaims.Should().HaveCount(2);
        permissionClaims.Select(c => c.Value).Should().Contain(new[]
        {
            "allow:read:course:123",
            "deny:delete:user:789"
        });
    }

    [Fact]
    public async Task HandleAuthenticateAsync_ShouldSucceedWithoutRolesOrPermissionsAsync()
    {
        // Arrange
        _context.Request.Headers[HeaderNames.UserIdHeader] = "user123";
        var handler = CreateHandler();

        // Act
        var result = await handler.AuthenticateAsync();

        // Assert
        result.Succeeded.Should().BeTrue();
        result.Principal!.FindAll(ClaimTypes.Role).Should().BeEmpty();
        result.Principal!.FindAll(PermissionClaim.ClaimType).Should().BeEmpty();
    }

    [Fact]
    public async Task HandleAuthenticateAsync_ShouldHandleNullRoleValuesAsync()
    {
        // Arrange
        _context.Request.Headers[HeaderNames.UserIdHeader] = "user123";
        _context.Request.Headers[HeaderNames.UserRoleHeader] = default(string);
        var handler = CreateHandler();

        // Act
        var result = await handler.AuthenticateAsync();

        // Assert
        result.Succeeded.Should().BeTrue();
        result.Principal!.FindAll(ClaimTypes.Role).Should().BeEmpty();
    }

    [Fact]
    public async Task HandleAuthenticateAsync_ShouldHandleNullPermissionValuesAsync()
    {
        // Arrange
        _context.Request.Headers[HeaderNames.UserIdHeader] = "user123";
        _context.Request.Headers[HeaderNames.UserPermissionsHeader] = default(string);
        var handler = CreateHandler();

        // Act
        var result = await handler.AuthenticateAsync();

        // Assert
        result.Succeeded.Should().BeTrue();
        result.Principal!.FindAll(PermissionClaim.ClaimType).Should().BeEmpty();
    }

    [Fact]
    public async Task HandleAuthenticateAsync_ShouldCreateCorrectAuthenticationTicketAsync()
    {
        // Arrange
        _context.Request.Headers[HeaderNames.UserIdHeader] = "user123";
        _context.Request.Headers[HeaderNames.UserRoleHeader] = "Admin";
        _context.Request.Headers[HeaderNames.UserPermissionsHeader] = "allow:read:course:*";
        var handler = CreateHandler();

        // Act
        var result = await handler.AuthenticateAsync();

        // Assert
        result.Succeeded.Should().BeTrue();
        result.Ticket.Should().NotBeNull();
        result.Ticket!.AuthenticationScheme.Should().Be(GatewayHeaderAuthenticationHandler.SchemeName);
        result.Ticket.Principal.Should().NotBeNull();
        result.Ticket.Principal.Identity!.AuthenticationType.Should().Be(GatewayHeaderAuthenticationHandler.SchemeName);
    }
}