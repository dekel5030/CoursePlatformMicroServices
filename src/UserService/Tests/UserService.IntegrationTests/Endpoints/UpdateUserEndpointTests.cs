using Application.Abstractions.Data;
using Auth.Contracts.Events;
using Domain.Users.Primitives;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace UserService.IntegrationTests.Endpoints;

public class UpdateUserEndpointTests : IntegrationTestsBase
{
    [Fact]
    public async Task UpdateUser_ShouldReturnOk_WhenUserExists()
    {
        // Arrange - Create a user first
        string authUserId = Guid.NewGuid().ToString();
        string userId = authUserId;
        string email = "testuser@example.com";

        var bus = Factory.Services.GetRequiredService<IBus>();
        await bus.Publish(new UserRegistered(authUserId, userId, email, DateTime.UtcNow));
        await Task.Delay(TimeSpan.FromSeconds(2)); // Wait for event processing

        var updateRequest = new
        {
            FirstName = "John",
            LastName = "Doe",
            PhoneNumber = new { CountryCode = "+1", Number = "1234567890" },
            DateOfBirth = new DateTime(1990, 1, 1)
        };

        // Act
        var response = await Client.PutAsJsonAsync($"/users/{userId}", updateRequest);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<UpdatedUserResponse>();
        Assert.NotNull(result);
        Assert.Equal("John", result.FirstName);
        Assert.Equal("Doe", result.LastName);
        Assert.Equal("+1 1234567890", result.PhoneNumber);
    }

    [Fact]
    public async Task UpdateUser_ShouldReturnNotFound_WhenUserDoesNotExist()
    {
        // Arrange
        var nonExistentUserId = Guid.NewGuid();
        var updateRequest = new
        {
            FirstName = "John",
            LastName = "Doe"
        };

        // Act
        var response = await Client.PutAsJsonAsync($"/users/{nonExistentUserId}", updateRequest);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateUser_ShouldUpdatePartialFields_WhenOnlySomeFieldsProvided()
    {
        // Arrange - Create a user first
        string authUserId = Guid.NewGuid().ToString();
        string userId = authUserId;
        string email = "partial@example.com";

        var bus = Factory.Services.GetRequiredService<IBus>();
        await bus.Publish(new UserRegistered(authUserId, userId, email, DateTime.UtcNow));
        await Task.Delay(TimeSpan.FromSeconds(2));

        var updateRequest = new
        {
            FirstName = "Jane",
            LastName = (string?)null,
            PhoneNumber = (object?)null,
            DateOfBirth = (DateTime?)null
        };

        // Act
        var response = await Client.PutAsJsonAsync($"/users/{userId}", updateRequest);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    private record UpdatedUserResponse(
        Guid Id,
        string Email,
        string? FirstName,
        string? LastName,
        DateTime? DateOfBirth,
        string? PhoneNumber);
}
