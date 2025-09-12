using Application.Abstractions.Data;
using Domain.Users.Primitives;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Users.Contracts.Events;
using Xunit;

namespace OrderService.IntegrationTests.Consumers;
public class UserUpsertedConsumerTest: IntegrationTestsBase
{
    private readonly string _userId = Guid.NewGuid().ToString();
    private readonly string _email = "test@test.com";
    private readonly string _fullname = "Test User";
    private readonly bool _isActive = true;
    private readonly long _entityVersion = 1;

    private static async Task Eventually(Func<Task<bool>> condition, int timeoutMs = 500000, int intervalMs = 200)
    {
        var start = DateTime.UtcNow;
        while (DateTime.UtcNow - start < TimeSpan.FromMilliseconds(timeoutMs))
        {
            if (await condition())
                return;

            await Task.Delay(intervalMs);
        }

        throw new TimeoutException("Condition not met within timeout.");
    }

    [Fact]
    public async Task Consume_Should_SkipDuplicateEvent()
    {
        // Arrange
        var userContract = new UserUpsertedV1(
            UserId: _userId,
            Email: _email,
            Fullname: _fullname,
            IsActive: _isActive,
            EntityVersion: _entityVersion
        );

        using var scope1 = Factory.Services.CreateScope();
        var dbContext1 = scope1.ServiceProvider.GetRequiredService<IReadDbContext>();

        await ExternalBus.Publish(userContract);
        await ExternalBus.Publish(userContract);

        // Assert
        await Eventually(async () =>
        {
            using var scope = Factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<IReadDbContext>();

            var user = await db.Users.FirstOrDefaultAsync(u => u.ExternalUserId == new ExternalUserId(_userId));
            if (user is null) return false;

            user.EntityVersion.Should().Be(1);

            return user.EntityVersion == 1;
        });
    }

    [Fact]
    public async Task Consume_Should_ApplyEventsInAggregateOrder_NotPublishOrder()
    {
        // Arrange
        var v1 = new UserUpsertedV1(_userId, "first@email.com", "first name", true, 1);
        var v2 = new UserUpsertedV1(_userId, "second@email.com", "second name", true, 2);
        var v3 = new UserUpsertedV1(_userId, "second@email.com", "second name", false, 3);

        await ExternalBus.Publish(v3);
        await ExternalBus.Publish(v2);
        await ExternalBus.Publish(v1);

        // מחכים כמה שניות כדי לאפשר ל־Consumer לעבד
        await Task.Delay(TimeSpan.FromMinutes(1));

        // Assert
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<IReadDbContext>();
        var final = await db.Users.SingleAsync(u => u.ExternalUserId == new ExternalUserId(_userId));

        final.Email.Should().Be("second@email.com");
        final.Fullname.Should().Be("second name");
        final.IsActive.Should().BeFalse();
        final.EntityVersion.Should().Be(3);
    }



}
