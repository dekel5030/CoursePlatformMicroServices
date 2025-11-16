using Application.Abstractions.Data;
using Auth.Contracts;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace OrderService.IntegrationTests.Consumers;

public class AuthRegisterdConsumerTest : IntegrationTestsBase
{
    [Fact]
    public async Task Consume_ShouldCreateUser_WhenNotExists()
    {
        // Arrange
        string email = "user@example.com";

        // Act
        var bus = Factory.Services.GetRequiredService<IBus>();
        await bus.Publish(new AuthRegistered(email, null));

        await Task.Delay(TimeSpan.FromMinutes(1));

        // Assert
        await Eventually(async () =>
        {
            using var scope = Factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<IReadDbContext>();

            var user = await db.Users.FirstOrDefaultAsync(u => u.Email == email);

            return user != null;
        });
    }

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
}
