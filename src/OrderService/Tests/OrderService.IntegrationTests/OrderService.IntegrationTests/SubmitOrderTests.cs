using Domain.Orders;
using Domain.Orders.Primitives;
using Domain.Products;
using Domain.Products.Primitives;
using Domain.Users;
using Domain.Users.Primitives;
using FluentAssertions;
using Infrastructure.Database;
using MassTransit.EntityFrameworkCoreIntegration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using SharedKernel;
using System.Net.Http.Json;
using System.Text;
using Xunit;

namespace OrderService.IntegrationTests;

public record CreatedOrderResponse(Guid Id);

public class SubmitOrderTests : IntegrationTestsBase
{
    private readonly User _user;
    private readonly Product _product1;
    private readonly Product _product2;

    public SubmitOrderTests()
    {
        string userId = "11111111-1111-1111-1111-111111111111";

        _user = User.Create(new ExternalUserId(userId), "test@gmail.com", "test", true);

        string productId1 = "22222222-2222-2222-2222-222222222222";
        string productId2 = "33333333-3333-3333-3333-333333333333";

        _product1 = Product.Create(new ExternalProductId(productId1), "Product 1", Money.Zero());
        _product2 = Product.Create(new ExternalProductId(productId2), "Product 2", Money.Zero());

    }

    private async Task SeedData()
    {
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<WriteDbContext>();

        await dbContext.Users.AddAsync(_user);
        await dbContext.Products.AddRangeAsync(_product1, _product2);
        await dbContext.SaveChangesAsync();
    }

    [Fact]
    public async Task SubmitOrder_Should_WriteOrder()
    {
        // Arrange
        await SeedData();

        var request = new
        {
            ExternalUserId = _user.ExternalUserId.Value,
            Products = new[]
            {
                new { ExternalId = _product1.ExternalId.Value, Quantity = 2 },
                new { ExternalId = _product2.ExternalId.Value, Quantity = 1 }
            }
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/orders", request);
        var created = await response.Content.ReadFromJsonAsync<CreatedOrderResponse>();
        var orderId = new OrderId(created!.Id);

        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<WriteDbContext>();
        Order? order = await dbContext.Orders
            .Include(o => o.Lines)
            .FirstOrDefaultAsync(o => o.Id == orderId);

        // Assert
        response.EnsureSuccessStatusCode();
        order.Should().NotBeNull();
        order.ExternalUserId.Should().Be(_user.ExternalUserId);
        order.Lines.Should().HaveCount(2);
    }

    [Fact]
    public async Task SubmitOrder_Should_WriteOutboxMessage()
    {
        // Arrange
        await SeedData();

        var request = new
        {
            ExternalUserId = _user.ExternalUserId.Value,
            Products = new[]
            {
                new { ExternalId = _product1.ExternalId.Value, Quantity = 2 },
                new { ExternalId = _product2.ExternalId.Value, Quantity = 1 }
            }
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/orders", request);
        response.EnsureSuccessStatusCode();

        var created = await response.Content.ReadFromJsonAsync<CreatedOrderResponse>();
        var orderId = new OrderId(created!.Id);

        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<WriteDbContext>();

        // Assert
        var outboxMessage = await dbContext.Set<OutboxMessage>()
            .OrderByDescending(m => m.EnqueueTime)
            .FirstOrDefaultAsync();

        outboxMessage.Should().NotBeNull();

        var body = outboxMessage.Body;
        body.Should().Contain("OrderSubmitted");
        body.Should().Contain(orderId.Value.ToString());
    }

    [Fact]
    public async Task SubmitOrder_Should_Store_And_Deliver_OutboxMessage()
    {
        // Arrange
        await SeedData();

        var request = new
        {
            ExternalUserId = _user.ExternalUserId.Value,
            Products = new[]
            {
                new { ExternalId = _product1.ExternalId.Value, Quantity = 2 }
            }
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/orders", request);
        response.EnsureSuccessStatusCode();

        var created = await response.Content.ReadFromJsonAsync<CreatedOrderResponse>();
        var orderId = new OrderId(created!.Id);

        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<WriteDbContext>();

        var insertedMessage = await dbContext.Set<OutboxMessage>()
            .OrderByDescending(m => m.SentTime)
            .FirstOrDefaultAsync();

        insertedMessage.Should().NotBeNull("because an OrderSubmitted event should be stored in the outbox");
        insertedMessage!.MessageType.Should().Contain("OrderSubmitted");
        insertedMessage.Body.Should().Contain(orderId.Value.ToString());

        OutboxMessage? pendingMessage = insertedMessage;
        for (int i = 0; i < 20 && pendingMessage != null; i++)
        {
            await Task.Delay(500);

            pendingMessage = await dbContext.Set<OutboxMessage>()
                .FirstOrDefaultAsync(m => m.MessageId == insertedMessage.MessageId);
        }

        pendingMessage.Should().BeNull("because the Outbox Delivery Service should have delivered the message");

        var remainingMessages = await dbContext.Set<OutboxMessage>().ToListAsync();
        remainingMessages.Should().BeEmpty("because the Outbox Delivery Service should have delivered all messages");

    }
}