using System.Text.Json;
using Auth.Contracts.Redis;
using FluentAssertions;
using Infrastructure.Redis;
using Moq;
using StackExchange.Redis;
using Xunit;

namespace Infrastructure.UnitTests.Redis;

/// <summary>
/// Unit tests for RedisRolePermissionsCacheWriter.
/// Verifies that role permissions are correctly serialized and written to Redis cache.
/// </summary>
public class RedisRolePermissionsCacheWriterTests
{
    private readonly Mock<IConnectionMultiplexer> _mockMultiplexer;
    private readonly Mock<IDatabase> _mockDatabase;
    private readonly RedisRolePermissionsCacheWriter _sut;

    public RedisRolePermissionsCacheWriterTests()
    {
        _mockMultiplexer = new Mock<IConnectionMultiplexer>();
        _mockDatabase = new Mock<IDatabase>();
        
        _mockMultiplexer
            .Setup(m => m.GetDatabase(It.IsAny<int>(), It.IsAny<object>()))
            .Returns(_mockDatabase.Object);
        
        _sut = new RedisRolePermissionsCacheWriter(_mockMultiplexer.Object);
    }

    [Fact]
    public async Task UpdateAsync_WithValidRoleAndPermissions_ShouldCallStringSetAsync()
    {
        // Arrange
        var roleName = "Admin";
        var permissions = new List<string> { "allow:read:course:*", "allow:write:user:*" };
        var cancellationToken = CancellationToken.None;

        _mockDatabase
            .Setup(db => db.StringSetAsync(
                It.IsAny<RedisKey>(),
                It.IsAny<RedisValue>(),
                It.IsAny<TimeSpan?>(),
                It.IsAny<bool>(),
                It.IsAny<When>(),
                It.IsAny<CommandFlags>()))
            .ReturnsAsync(true);

        // Act
        await _sut.UpdateAsync(roleName, permissions, cancellationToken);

        // Assert
        _mockDatabase.Verify(
            db => db.StringSetAsync(
                It.IsAny<RedisKey>(),
                It.IsAny<RedisValue>(),
                It.IsAny<TimeSpan?>(),
                It.IsAny<bool>(),
                It.IsAny<When>(),
                It.IsAny<CommandFlags>()),
            Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUseCorrectRedisKey()
    {
        // Arrange
        var roleName = "Moderator";
        var permissions = new List<string> { "allow:read:course:*" };
        var cancellationToken = CancellationToken.None;
        var expectedKey = RedisKeys.RolePermissions(roleName);
        RedisKey? capturedKey = null;

        _mockDatabase
            .Setup(db => db.StringSetAsync(
                It.IsAny<RedisKey>(),
                It.IsAny<RedisValue>(),
                It.IsAny<TimeSpan?>(),
                It.IsAny<bool>(),
                It.IsAny<When>(),
                It.IsAny<CommandFlags>()))
            .Callback<RedisKey, RedisValue, TimeSpan?, bool, When, CommandFlags>(
                (key, value, expiry, keepTtl, when, flags) => capturedKey = key)
            .ReturnsAsync(true);

        // Act
        await _sut.UpdateAsync(roleName, permissions, cancellationToken);

        // Assert
        capturedKey.Should().NotBeNull();
        capturedKey.ToString().Should().Be(expectedKey);
    }

    [Fact]
    public async Task UpdateAsync_ShouldSerializeModelToJson()
    {
        // Arrange
        var roleName = "User";
        var permissions = new List<string> { "allow:read:course:123", "allow:update:lesson:456" };
        var cancellationToken = CancellationToken.None;
        RedisValue? capturedValue = null;

        _mockDatabase
            .Setup(db => db.StringSetAsync(
                It.IsAny<RedisKey>(),
                It.IsAny<RedisValue>(),
                It.IsAny<TimeSpan?>(),
                It.IsAny<bool>(),
                It.IsAny<When>(),
                It.IsAny<CommandFlags>()))
            .Callback<RedisKey, RedisValue, TimeSpan?, bool, When, CommandFlags>(
                (key, value, expiry, keepTtl, when, flags) => capturedValue = value)
            .ReturnsAsync(true);

        // Act
        await _sut.UpdateAsync(roleName, permissions, cancellationToken);

        // Assert
        capturedValue.Should().NotBeNull();
        var deserializedModel = JsonSerializer.Deserialize<RolePermissionsCacheModel>(capturedValue!);
        deserializedModel.Should().NotBeNull();
        deserializedModel!.RoleName.Should().Be(roleName);
        deserializedModel.Permissions.Should().BeEquivalentTo(permissions);
    }

    [Fact]
    public async Task UpdateAsync_WithEmptyPermissions_ShouldStoreEmptyCollection()
    {
        // Arrange
        var roleName = "Guest";
        var permissions = new List<string>();
        var cancellationToken = CancellationToken.None;
        RedisValue? capturedValue = null;

        _mockDatabase
            .Setup(db => db.StringSetAsync(
                It.IsAny<RedisKey>(),
                It.IsAny<RedisValue>(),
                It.IsAny<TimeSpan?>(),
                It.IsAny<bool>(),
                It.IsAny<When>(),
                It.IsAny<CommandFlags>()))
            .Callback<RedisKey, RedisValue, TimeSpan?, bool, When, CommandFlags>(
                (key, value, expiry, keepTtl, when, flags) => capturedValue = value)
            .ReturnsAsync(true);

        // Act
        await _sut.UpdateAsync(roleName, permissions, cancellationToken);

        // Assert
        capturedValue.Should().NotBeNull();
        var deserializedModel = JsonSerializer.Deserialize<RolePermissionsCacheModel>(capturedValue!);
        deserializedModel.Should().NotBeNull();
        deserializedModel!.RoleName.Should().Be(roleName);
        deserializedModel.Permissions.Should().BeEmpty();
    }

    [Theory]
    [InlineData("Admin")]
    [InlineData("User")]
    [InlineData("Moderator")]
    [InlineData("Guest")]
    public async Task UpdateAsync_WithDifferentRoleNames_ShouldUseLowerCaseInKey(string roleName)
    {
        // Arrange
        var permissions = new List<string> { "allow:read:course:*" };
        var cancellationToken = CancellationToken.None;
        RedisKey? capturedKey = null;

        _mockDatabase
            .Setup(db => db.StringSetAsync(
                It.IsAny<RedisKey>(),
                It.IsAny<RedisValue>(),
                It.IsAny<TimeSpan?>(),
                It.IsAny<bool>(),
                It.IsAny<When>(),
                It.IsAny<CommandFlags>()))
            .Callback<RedisKey, RedisValue, TimeSpan?, bool, When, CommandFlags>(
                (key, value, expiry, keepTtl, when, flags) => capturedKey = key)
            .ReturnsAsync(true);

        // Act
        await _sut.UpdateAsync(roleName, permissions, cancellationToken);

        // Assert
        capturedKey.Should().NotBeNull();
        capturedKey.ToString().Should().Be($"auth:roles:{roleName.ToLowerInvariant()}");
    }

    [Fact]
    public async Task UpdateAsync_WithMultiplePermissions_ShouldStoreAllPermissions()
    {
        // Arrange
        var roleName = "SuperAdmin";
        var permissions = new List<string>
        {
            "allow:create:course:*",
            "allow:read:course:*",
            "allow:update:course:*",
            "allow:delete:course:*",
            "allow:create:user:*",
            "allow:delete:user:*"
        };
        var cancellationToken = CancellationToken.None;
        RedisValue? capturedValue = null;

        _mockDatabase
            .Setup(db => db.StringSetAsync(
                It.IsAny<RedisKey>(),
                It.IsAny<RedisValue>(),
                It.IsAny<TimeSpan?>(),
                It.IsAny<bool>(),
                It.IsAny<When>(),
                It.IsAny<CommandFlags>()))
            .Callback<RedisKey, RedisValue, TimeSpan?, bool, When, CommandFlags>(
                (key, value, expiry, keepTtl, when, flags) => capturedValue = value)
            .ReturnsAsync(true);

        // Act
        await _sut.UpdateAsync(roleName, permissions, cancellationToken);

        // Assert
        capturedValue.Should().NotBeNull();
        var deserializedModel = JsonSerializer.Deserialize<RolePermissionsCacheModel>(capturedValue!);
        deserializedModel.Should().NotBeNull();
        deserializedModel!.Permissions.Should().HaveCount(6);
        deserializedModel.Permissions.Should().BeEquivalentTo(permissions);
    }

    [Fact]
    public async Task UpdateAsync_ShouldPreservePermissionOrder()
    {
        // Arrange
        var roleName = "Editor";
        var permissions = new List<string>
        {
            "allow:create:lesson:*",
            "allow:read:lesson:*",
            "allow:update:lesson:*"
        };
        var cancellationToken = CancellationToken.None;
        RedisValue? capturedValue = null;

        _mockDatabase
            .Setup(db => db.StringSetAsync(
                It.IsAny<RedisKey>(),
                It.IsAny<RedisValue>(),
                It.IsAny<TimeSpan?>(),
                It.IsAny<bool>(),
                It.IsAny<When>(),
                It.IsAny<CommandFlags>()))
            .Callback<RedisKey, RedisValue, TimeSpan?, bool, When, CommandFlags>(
                (key, value, expiry, keepTtl, when, flags) => capturedValue = value)
            .ReturnsAsync(true);

        // Act
        await _sut.UpdateAsync(roleName, permissions, cancellationToken);

        // Assert
        capturedValue.Should().NotBeNull();
        var deserializedModel = JsonSerializer.Deserialize<RolePermissionsCacheModel>(capturedValue!);
        deserializedModel.Should().NotBeNull();
        deserializedModel!.Permissions.Should().ContainInOrder(permissions);
    }
}
