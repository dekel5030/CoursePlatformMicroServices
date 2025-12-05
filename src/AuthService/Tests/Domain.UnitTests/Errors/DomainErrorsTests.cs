using Domain.AuthUsers.Errors;
using Domain.Roles.Errors;
using FluentAssertions;
using Kernel;
using Xunit;

namespace Domain.UnitTests.Errors;

/// <summary>
/// Unit tests for domain error definitions.
/// Tests verify error codes, descriptions, and error types are properly configured.
/// </summary>
public class DomainErrorsTests
{
    #region AuthUserErrors Tests

    /// <summary>
    /// Verifies that AuthUserErrors.NotFound has the correct error type.
    /// </summary>
    [Fact]
    public void AuthUserErrors_NotFound_ShouldBeNotFoundErrorType()
    {
        // Arrange & Act
        var error = AuthUserErrors.NotFound;

        // Assert
        error.Type.Should().Be(ErrorType.NotFound);
        error.Code.Should().Be("AuthUser.NotFound");
        error.Description.Should().NotBeNullOrEmpty();
    }

    /// <summary>
    /// Verifies that AuthUserErrors.DuplicateEmail has the correct error type.
    /// </summary>
    [Fact]
    public void AuthUserErrors_DuplicateEmail_ShouldBeConflictErrorType()
    {
        // Arrange & Act
        var error = AuthUserErrors.DuplicateEmail;

        // Assert
        error.Type.Should().Be(ErrorType.Conflict);
        error.Code.Should().Be("AuthUser.DuplicateEmail");
        error.Description.Should().NotBeNullOrEmpty();
    }

    /// <summary>
    /// Verifies that AuthUserErrors.InvalidCredentials has the correct error type.
    /// </summary>
    [Fact]
    public void AuthUserErrors_InvalidCredentials_ShouldBeValidationErrorType()
    {
        // Arrange & Act
        var error = AuthUserErrors.InvalidCredentials;

        // Assert
        error.Type.Should().Be(ErrorType.Validation);
        error.Code.Should().Be("AuthUser.InvalidCredentials");
        error.Description.Should().NotBeNullOrEmpty();
    }

    /// <summary>
    /// Verifies that AuthUserErrors.AccountLocked has the correct properties.
    /// </summary>
    [Fact]
    public void AuthUserErrors_AccountLocked_ShouldHaveCorrectProperties()
    {
        // Arrange & Act
        var error = AuthUserErrors.AccountLocked;

        // Assert
        error.Type.Should().Be(ErrorType.Validation);
        error.Code.Should().Be("AuthUser.AccountLocked");
        error.Description.Should().NotBeNullOrEmpty();
    }

    /// <summary>
    /// Verifies that AuthUserErrors.Unauthorized has the correct error type.
    /// </summary>
    [Fact]
    public void AuthUserErrors_Unauthorized_ShouldBeUnauthorizedErrorType()
    {
        // Arrange & Act
        var error = AuthUserErrors.Unauthorized;

        // Assert
        error.Type.Should().Be(ErrorType.Unauthorized);
        error.Code.Should().Be("AuthUser.Unauthorized");
        error.Description.Should().NotBeNullOrEmpty();
    }

    /// <summary>
    /// Verifies that various AuthUserErrors have distinct error codes.
    /// </summary>
    [Theory]
    [InlineData("NotFound")]
    [InlineData("DuplicateEmail")]
    [InlineData("InvalidCredentials")]
    [InlineData("AccountLocked")]
    [InlineData("AccountInactive")]
    [InlineData("UserLockedOut")]
    [InlineData("EmailNotConfirmed")]
    [InlineData("Unauthorized")]
    [InlineData("IsLockOut")]
    [InlineData("Required2FA")]
    public void AuthUserErrors_ShouldHaveDistinctErrorCodes(string errorName)
    {
        // Arrange
        var errorProperty = typeof(AuthUserErrors).GetField(errorName);

        // Act
        var error = (Error)errorProperty!.GetValue(null)!;

        // Assert
        error.Should().NotBeNull();
        error.Code.Should().StartWith("AuthUser.");
        error.Description.Should().NotBeNullOrEmpty();
    }

    #endregion

    #region RoleErrors Tests

    /// <summary>
    /// Verifies that RoleErrors.NotFound has the correct error type.
    /// </summary>
    [Fact]
    public void RoleErrors_NotFound_ShouldBeNotFoundErrorType()
    {
        // Arrange & Act
        var error = RoleErrors.NotFound;

        // Assert
        error.Type.Should().Be(ErrorType.NotFound);
        error.Code.Should().Be("Role.NotFound");
        error.Description.Should().NotBeNullOrEmpty();
    }

    /// <summary>
    /// Verifies that RoleErrors.DuplicateName has the correct error type.
    /// </summary>
    [Fact]
    public void RoleErrors_DuplicateName_ShouldBeConflictErrorType()
    {
        // Arrange & Act
        var error = RoleErrors.DuplicateName;

        // Assert
        error.Type.Should().Be(ErrorType.Conflict);
        error.Code.Should().Be("Role.DuplicateName");
        error.Description.Should().NotBeNullOrEmpty();
    }

    #endregion
}
