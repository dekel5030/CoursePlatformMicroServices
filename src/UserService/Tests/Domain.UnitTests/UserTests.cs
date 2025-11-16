using Domain.Users;
using Domain.Users.Events;
using Domain.Users.Primitives;
using FluentAssertions;
using Xunit;

namespace Domain.UnitTests;

public class UserTests
{
    [Fact]
    public void CreateUser_WithAuthUserId_ShouldSucceed()
    {
        // Arrange
        var authUserId = new AuthUserId(Guid.NewGuid().ToString());
        var email = "test@example.com";

        // Act
        var result = User.CreateUser(authUserId, email);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.AuthUserId.Should().Be(authUserId);
        result.Value.Email.Should().Be(email);
        result.Value.Id.Value.Should().NotBeEmpty();
    }

    [Fact]
    public void CreateUser_ShouldRaiseUserProfileCreatedDomainEvent()
    {
        // Arrange
        var authUserId = new AuthUserId(Guid.NewGuid().ToString());
        var email = "test@example.com";

        // Act
        var result = User.CreateUser(authUserId, email);

        // Assert
        result.IsSuccess.Should().BeTrue();
        var user = result.Value;
        
        // Check that domain event is raised
        var domainEvents = user.DomainEvents;
        domainEvents.Should().HaveCount(1);
        domainEvents.First().Should().BeOfType<UserProfileCreatedDomainEvent>();
        
        var domainEvent = (UserProfileCreatedDomainEvent)domainEvents.First();
        domainEvent.UserId.Should().Be(user.Id);
        domainEvent.AuthUserId.Should().Be(authUserId);
        domainEvent.Email.Should().Be(email);
    }

    [Fact]
    public void CreateUser_WithOptionalFields_ShouldSucceed()
    {
        // Arrange
        var authUserId = new AuthUserId(Guid.NewGuid().ToString());
        var email = "test@example.com";
        var fullName = new FullName("John", "Doe");
        var phoneNumber = new PhoneNumber("+1", "234567890");
        var dateOfBirth = new DateTime(1990, 1, 1);

        // Act
        var result = User.CreateUser(
            authUserId, 
            email, 
            fullName, 
            phoneNumber, 
            dateOfBirth);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.FullName.Should().Be(fullName);
        result.Value.PhoneNumber.Should().Be(phoneNumber);
        result.Value.DateOfBirth.Should().Be(dateOfBirth);
    }
}
