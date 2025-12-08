//using FluentAssertions;
//using Kernel.Auth;
//using Kernel.Auth.AuthTypes;

//namespace Kernel.UnitTests.Auth;

//public class PermissionClaimTests
//{
//    [Fact]
//    public void Create_ShouldNormalizeToLowercase()
//    {
//        // Arrange & Act
//        // תיקון: יצירת ResourceId באמצעות ה-Factory
//        var claim = PermissionClaim.Create(
//            EffectType.Allow,
//            ActionType.Read,
//            ResourceType.Course,
//            ResourceId.Create("ABC123"));

//        // Assert
//        claim.Type.Should().Be("cp_permission");
//        claim.Value.Should().Be("allow:read:course:abc123");
//    }

//    [Fact]
//    public void Create_ShouldHandleWildcardAction()
//    {
//        // Arrange & Act
//        var claim = PermissionClaim.Create(
//            EffectType.Allow,
//            ActionType.Wildcard,
//            ResourceType.Course,
//            ResourceId.Create("123"));

//        // Assert
//        claim.Value.Should().Be("allow:*:course:123");
//    }

//    [Fact]
//    public void Create_ShouldHandleWildcardResource()
//    {
//        // Arrange & Act
//        var claim = PermissionClaim.Create(
//            EffectType.Allow,
//            ActionType.Read,
//            ResourceType.Wildcard,
//            ResourceId.Create("123"));

//        // Assert
//        claim.Value.Should().Be("allow:read:*:123");
//    }

//    [Fact]
//    public void Create_ShouldHandleWildcardId()
//    {
//        // Arrange & Act
//        // תיקון: שימוש ב-ResourceId.Wildcard
//        var claim = PermissionClaim.Create(
//            EffectType.Allow,
//            ActionType.Read,
//            ResourceType.Course,
//            ResourceId.Wildcard);

//        // Assert
//        claim.Value.Should().Be("allow:read:course:*");
//    }

//    [Fact]
//    public void ResourceId_Create_ShouldThrowWhenIdContainsColon()
//    {
//        // Arrange & Act
//        // תיקון: הבדיקה עוברת ל-ResourceId.Create כי הוא זה שעושה את הוולידציה עכשיו
//        var action = () => ResourceId.Create("invalid:id");

//        // Assert
//        action.Should().Throw<ArgumentException>()
//            .WithMessage("Resource ID cannot contain the delimiter ':'.");
//    }

//    [Fact]
//    public void Create_ShouldAllowWildcardIdEvenThoughItContainsNoColon()
//    {
//        // Arrange & Act
//        var claim = PermissionClaim.Create(
//            EffectType.Deny,
//            ActionType.Delete,
//            ResourceType.User,
//            ResourceId.Wildcard);

//        // Assert
//        claim.Value.Should().Be("deny:delete:user:*");
//    }

//    [Fact]
//    public void Parse_ShouldParseValidClaimValue()
//    {
//        // Arrange
//        var claimValue = "allow:read:course:123";

//        // Act
//        var claim = PermissionClaim.Parse(claimValue);

//        // Assert
//        claim.Type.Should().Be("cp_permission");
//        claim.Value.Should().Be("allow:read:course:123");
//    }

//    [Fact]
//    public void Parse_ShouldHandleWildcardAction()
//    {
//        // Arrange
//        var claimValue = "allow:*:course:123";

//        // Act
//        var claim = PermissionClaim.Parse(claimValue);

//        // Assert
//        claim.Value.Should().Be("allow:*:course:123");
//    }

//    [Fact]
//    public void Parse_ShouldHandleWildcardResource()
//    {
//        // Arrange
//        var claimValue = "allow:read:*:123";

//        // Act
//        var claim = PermissionClaim.Parse(claimValue);

//        // Assert
//        claim.Value.Should().Be("allow:read:*:123");
//    }

//    [Fact]
//    public void Parse_ShouldHandleWildcardId()
//    {
//        // Arrange
//        var claimValue = "deny:delete:user:*";

//        // Act
//        var claim = PermissionClaim.Parse(claimValue);

//        // Assert
//        claim.Value.Should().Be("deny:delete:user:*");
//    }

//    [Fact]
//    public void Parse_ShouldThrowOnInvalidSegmentCount()
//    {
//        // Arrange
//        var claimValue = "allow:read:course"; // Missing ID segment entirely

//        // Act
//        var action = () => PermissionClaim.Parse(claimValue);

//        // Assert
//        action.Should().Throw<ArgumentException>()
//            .WithMessage("Invalid permission claim format.");
//    }

//    [Fact]
//    public void Parse_ShouldThrowOnTooManySegments()
//    {
//        // Arrange
//        var claimValue = "allow:read:course:123:extra";

//        // Act
//        var action = () => PermissionClaim.Parse(claimValue);

//        // Assert
//        action.Should().Throw<ArgumentException>()
//            .WithMessage("Invalid permission claim format.");
//    }

//    [Fact]
//    public void Parse_ShouldThrowOnInvalidEffect()
//    {
//        // Arrange
//        var claimValue = "invalid:read:course:123";

//        // Act
//        var action = () => PermissionClaim.Parse(claimValue);

//        // Assert
//        action.Should().Throw<ArgumentException>()
//            .WithMessage("Invalid effect value.");
//    }

//    [Fact]
//    public void Parse_ShouldThrowOnInvalidAction()
//    {
//        // Arrange
//        var claimValue = "allow:invalidaction:course:123";

//        // Act
//        var action = () => PermissionClaim.Parse(claimValue);

//        // Assert
//        action.Should().Throw<ArgumentException>()
//            .WithMessage("Invalid action value.");
//    }

//    [Fact]
//    public void Parse_ShouldThrowOnInvalidResource()
//    {
//        // Arrange
//        var claimValue = "allow:read:invalidresource:123";

//        // Act
//        var action = () => PermissionClaim.Parse(claimValue);

//        // Assert
//        action.Should().Throw<ArgumentException>()
//            .WithMessage("Invalid resource value.");
//    }

//    [Fact]
//    public void Parse_ShouldThrowOnEmptyId()
//    {
//        // Arrange
//        var claimValue = "allow:read:course:"; // ID is empty string

//        // Act
//        var action = () => PermissionClaim.Parse(claimValue);

//        // Assert
//        // ההודעה השתנתה כי הוולידציה בתוך ResourceId
//        action.Should().Throw<ArgumentException>()
//            .WithMessage("Resource ID cannot be empty.");
//    }

//    [Fact]
//    public void Parse_ShouldThrowOnWhitespaceId()
//    {
//        // Arrange
//        var claimValue = "allow:read:course:   ";

//        // Act
//        var action = () => PermissionClaim.Parse(claimValue);

//        // Assert
//        // ההודעה השתנתה כי הוולידציה בתוך ResourceId
//        action.Should().Throw<ArgumentException>()
//            .WithMessage("Resource ID cannot be empty.");
//    }

//    [Fact]
//    public void TryParse_ShouldReturnClaimForValidValue()
//    {
//        // Arrange
//        var claimValue = "allow:read:course:123";

//        // Act
//        var claim = PermissionClaim.TryParse(claimValue);

//        // Assert
//        claim.Should().NotBeNull();
//        claim!.Value.Should().Be("allow:read:course:123");
//    }

//    [Fact]
//    public void TryParse_ShouldReturnNullForInvalidValue()
//    {
//        // Arrange
//        var claimValue = "invalid";

//        // Act
//        var claim = PermissionClaim.TryParse(claimValue);

//        // Assert
//        claim.Should().BeNull();
//    }

//    [Fact]
//    public void TryParse_WithOutParameter_ShouldReturnTrueForValidValue()
//    {
//        // Arrange
//        var claimValue = "deny:update:user:456";

//        // Act
//        var result = PermissionClaim.TryParse(claimValue, out var claim);

//        // Assert
//        result.Should().BeTrue();
//        claim.Should().NotBeNull();
//        claim!.Value.Should().Be("deny:update:user:456");
//    }

//    [Fact]
//    public void TryParse_WithOutParameter_ShouldReturnFalseForInvalidValue()
//    {
//        // Arrange
//        var claimValue = "allow:read";

//        // Act
//        var result = PermissionClaim.TryParse(claimValue, out var claim);

//        // Assert
//        result.Should().BeFalse();
//        claim.Should().BeNull();
//    }

//    [Theory]
//    [InlineData("allow:*:*:*")]
//    [InlineData("deny:*:*:*")]
//    [InlineData("allow:create:*:*")]
//    [InlineData("allow:*:course:*")]
//    [InlineData("allow:*:*:123")]
//    public void Parse_ShouldHandleVariousWildcardCombinations(string claimValue)
//    {
//        // Act
//        var claim = PermissionClaim.Parse(claimValue);

//        // Assert
//        claim.Should().NotBeNull();
//        claim.Value.Should().Be(claimValue);
//    }

//    [Fact]
//    public void Create_ShouldProduceParsableValue()
//    {
//        // Arrange
//        var original = PermissionClaim.Create(
//            EffectType.Allow,
//            ActionType.Update,
//            ResourceType.Lesson,
//            ResourceId.Create("789")); // תיקון

//        // Act
//        var parsed = PermissionClaim.Parse(original.Value);

//        // Assert
//        parsed.Value.Should().Be(original.Value);
//    }
//}