using Users.Domain.Users;
using Users.Domain.Users.Events;
using Users.Domain.Users.Primitives;
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

    [Fact]
    public void UpdateProfile_WithNewFields_ShouldSucceed()
    {
        // Arrange
        var authUserId = new AuthUserId(Guid.NewGuid().ToString());
        var email = "test@example.com";
        var userResult = User.CreateUser(authUserId, email);
        var user = userResult.Value;

        var fullName = new FullName("Jane", "Smith");
        var avatarUrl = "https://example.com/avatar.jpg";
        var bio = "Software Engineer passionate about microservices";
        var linkedInUrl = "https://linkedin.com/in/janesmith";
        var gitHubUrl = "https://github.com/janesmith";

        // Act
        var result = user.UpdateProfile(
            fullName: fullName,
            avatarUrl: avatarUrl,
            bio: bio,
            linkedInUrl: linkedInUrl,
            gitHubUrl: gitHubUrl);

        // Assert
        result.IsSuccess.Should().BeTrue();
        user.FullName.Should().Be(fullName);
        user.AvatarUrl.Should().Be(avatarUrl);
        user.Bio.Should().Be(bio);
        user.LinkedInUrl.Should().Be(linkedInUrl);
        user.GitHubUrl.Should().Be(gitHubUrl);
    }

    [Fact]
    public void CreateLecturerProfile_ForNewUser_ShouldSucceed()
    {
        // Arrange
        var authUserId = new AuthUserId(Guid.NewGuid().ToString());
        var email = "lecturer@example.com";
        var userResult = User.CreateUser(authUserId, email);
        var user = userResult.Value;

        var professionalBio = "Experienced software architect with 15 years in the industry";
        var expertise = "Microservices, Cloud Architecture, .NET";
        var yearsOfExperience = 15;

        // Act
        var result = user.CreateLecturerProfile(professionalBio, expertise, yearsOfExperience);

        // Assert
        result.IsSuccess.Should().BeTrue();
        user.IsLecturer.Should().BeTrue();
        user.LecturerProfile.Should().NotBeNull();
        user.LecturerProfile!.ProfessionalBio.Should().Be(professionalBio);
        user.LecturerProfile.Expertise.Should().Be(expertise);
        user.LecturerProfile.YearsOfExperience.Should().Be(yearsOfExperience);
    }

    [Fact]
    public void CreateLecturerProfile_WhenProfileAlreadyExists_ShouldFail()
    {
        // Arrange
        var authUserId = new AuthUserId(Guid.NewGuid().ToString());
        var email = "lecturer@example.com";
        var userResult = User.CreateUser(authUserId, email);
        var user = userResult.Value;

        user.CreateLecturerProfile("First bio", "First expertise", 10);

        // Act
        var result = user.CreateLecturerProfile("Second bio", "Second expertise", 15);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("User.LecturerProfileExists");
    }
}
