using System.Security.Claims;
using FluentAssertions;
using Kernel.Auth;
using Kernel.Auth.AuthTypes;

namespace Kernel.UnitTests.Auth;

public class PermissionClaimTests
{
    #region Create Tests

    [Fact]
    public void Create_ShouldNormalizeToLowercase()
    {
        // Arrange & Act
        Claim claim = PermissionClaim.Create(
            EffectType.Allow,
            ActionType.Read,
            ResourceType.Course,
            ResourceId.Create("ABC123"));

        // Assert
        // כעת claim הוא מסוג System.Security.Claims.Claim
        claim.Type.Should().Be("cp_permission");
        claim.Value.Should().Be("allow:read:course:ABC123");
    }

    [Fact]
    public void Create_ShouldHandleWildcardAction()
    {
        // Arrange & Act
        Claim claim = PermissionClaim.Create(
            EffectType.Allow,
            ActionType.Wildcard,
            ResourceType.Course,
            ResourceId.Create("123"));

        // Assert
        claim.Value.Should().Be("allow:*:course:123");
    }

    [Fact]
    public void Create_ShouldHandleWildcardResource()
    {
        // Arrange & Act
        Claim claim = PermissionClaim.Create(
            EffectType.Allow,
            ActionType.Read,
            ResourceType.Wildcard,
            ResourceId.Create("123"));

        // Assert
        claim.Value.Should().Be("allow:read:*:123");
    }

    [Fact]
    public void Create_ShouldHandleWildcardId()
    {
        // Arrange & Act
        Claim claim = PermissionClaim.Create(
            EffectType.Allow,
            ActionType.Read,
            ResourceType.Course,
            ResourceId.Wildcard);

        // Assert
        claim.Value.Should().Be("allow:read:course:*");
    }

    [Fact]
    public void Create_ShouldAllowWildcardIdEvenThoughItContainsNoColon()
    {
        // Arrange & Act
        Claim claim = PermissionClaim.Create(
            EffectType.Deny,
            ActionType.Delete,
            ResourceType.User,
            ResourceId.Wildcard);

        // Assert
        claim.Value.Should().Be("deny:delete:user:*");
    }

    #endregion

    #region TryParse Tests (Replaces Parse Tests)

    // הערה: בקוד שלך אין פונקציית Parse שזורקת שגיאות, אלא TryParse שמחזירה false.
    // לכן שיניתי את הטסטים לבדוק הצלחה/כישלון בוליאני.

    [Fact]
    public void TryParse_ShouldReturnTrue_ForValidClaimValue()
    {
        // Arrange
        string claimValue = "allow:read:course:123";

        // Act
        bool result = PermissionClaim.TryParse(claimValue, out Claim? claim);

        // Assert
        result.Should().BeTrue();
        claim.Should().NotBeNull();
        claim.Type.Should().Be("cp_permission");
        claim.Value.Should().Be("allow:read:course:123");
    }

    [Theory]
    [InlineData("allow:*:course:123")]
    [InlineData("allow:read:*:123")]
    [InlineData("deny:delete:user:*")]
    public void TryParse_ShouldHandleWildcards(string claimValue)
    {
        // Act
        bool result = PermissionClaim.TryParse(claimValue, out Claim? claim);

        // Assert
        result.Should().BeTrue();
        claim.Value.Should().Be(claimValue);
    }

    [Theory]
    [InlineData("allow:*:*:*")]
    [InlineData("deny:*:*:*")]
    [InlineData("allow:create:*:*")]
    [InlineData("allow:*:course:*")]
    [InlineData("allow:*:*:123")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S4144:Methods should not have identical implementations", Justification = "<Pending>")]
    public void TryParse_ShouldHandleVariousWildcardCombinations(string claimValue)
    {
        // Act
        bool result = PermissionClaim.TryParse(claimValue, out Claim? claim);

        // Assert
        result.Should().BeTrue();
        claim.Value.Should().Be(claimValue);
    }

    [Fact]
    public void TryParse_ShouldReturnFalse_OnInvalidSegmentCount()
    {
        // Arrange
        string claimValue = "allow:read:course"; // Missing ID segment entirely

        // Act
        bool result = PermissionClaim.TryParse(claimValue, out Claim? claim);

        // Assert
        result.Should().BeFalse();
        claim.Should().BeNull();
    }

    [Fact]
    public void TryParse_ShouldReturnFalse_OnTooManySegments()
    {
        // Arrange
        string claimValue = "allow:read:course:123:extra";


        // Act
        bool result = PermissionClaim.TryParse(claimValue, out _);

        // Assert
        result.Should().BeFalse();
    }

    [Theory]
    [InlineData("invalid:read:course:123")] // Invalid Effect
    [InlineData("allow:invalidaction:course:123")] // Invalid Action
    [InlineData("allow:read:invalidresource:123")] // Invalid Resource
    public void TryParse_ShouldReturnFalse_OnInvalidEnums(string claimValue)
    {
        // Act
        bool result = PermissionClaim.TryParse(claimValue, out _);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void TryParse_ShouldReturnFalse_ForNullInput()
    {
        // Act
        bool result = PermissionClaim.TryParse(null!, out _);

        // Assert
        result.Should().BeFalse();
    }

    #endregion
}