using Users.Domain.Users;
using Users.Domain.Users.Primitives;
using FluentAssertions;
using Xunit;

namespace Domain.UnitTests;

public class LecturerProfileTests
{
    [Fact]
    public void CreateProfile_WithValidData_ShouldSucceed()
    {
        // Arrange
        var userId = new UserId(Guid.CreateVersion7());
        var professionalBio = "Experienced software architect";
        var expertise = "Microservices, Cloud Architecture";
        var yearsOfExperience = 15;

        // Act
        var result = LecturerProfile.CreateProfile(userId, professionalBio, expertise, yearsOfExperience);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.UserId.Should().Be(userId);
        result.Value.ProfessionalBio.Should().Be(professionalBio);
        result.Value.Expertise.Should().Be(expertise);
        result.Value.YearsOfExperience.Should().Be(yearsOfExperience);
        result.Value.Projects.Should().BeEmpty();
        result.Value.MediaItems.Should().BeEmpty();
        result.Value.Posts.Should().BeEmpty();
    }

    [Fact]
    public void UpdateProfile_WithNewInformation_ShouldSucceed()
    {
        // Arrange
        var userId = new UserId(Guid.CreateVersion7());
        var profileResult = LecturerProfile.CreateProfile(userId);
        var profile = profileResult.Value;

        var newBio = "Updated professional bio";
        var newExpertise = "Updated expertise";
        var newYears = 20;

        // Act
        var result = profile.UpdateProfile(newBio, newExpertise, newYears);

        // Assert
        result.IsSuccess.Should().BeTrue();
        profile.ProfessionalBio.Should().Be(newBio);
        profile.Expertise.Should().Be(newExpertise);
        profile.YearsOfExperience.Should().Be(newYears);
    }

    [Fact]
    public void AddProject_ShouldSucceed()
    {
        // Arrange
        var userId = new UserId(Guid.CreateVersion7());
        var profileResult = LecturerProfile.CreateProfile(userId);
        var profile = profileResult.Value;

        var project = Project.Create(
            "E-commerce Platform",
            "A scalable microservices-based e-commerce solution",
            "https://github.com/example/ecommerce",
            "https://example.com/thumbnail.jpg");

        // Act
        var result = profile.AddProject(project);

        // Assert
        result.IsSuccess.Should().BeTrue();
        profile.Projects.Should().HaveCount(1);
        profile.Projects.First().Title.Should().Be("E-commerce Platform");
    }

    [Fact]
    public void RemoveProject_WithExistingProject_ShouldSucceed()
    {
        // Arrange
        var userId = new UserId(Guid.CreateVersion7());
        var profileResult = LecturerProfile.CreateProfile(userId);
        var profile = profileResult.Value;

        var project = Project.Create("Test Project");
        profile.AddProject(project);

        // Act
        var result = profile.RemoveProject(project.Id);

        // Assert
        result.IsSuccess.Should().BeTrue();
        profile.Projects.Should().BeEmpty();
    }

    [Fact]
    public void RemoveProject_WithNonExistentProject_ShouldFail()
    {
        // Arrange
        var userId = new UserId(Guid.CreateVersion7());
        var profileResult = LecturerProfile.CreateProfile(userId);
        var profile = profileResult.Value;

        var nonExistentId = Guid.NewGuid();

        // Act
        var result = profile.RemoveProject(nonExistentId);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("LecturerProfile.ProjectNotFound");
    }

    [Fact]
    public void AddMediaItem_ShouldSucceed()
    {
        // Arrange
        var userId = new UserId(Guid.CreateVersion7());
        var profileResult = LecturerProfile.CreateProfile(userId);
        var profile = profileResult.Value;

        var mediaItem = MediaItem.Create(
            "https://example.com/image.jpg",
            MediaType.Image,
            "Conference Presentation",
            "Speaking at Tech Conference 2024");

        // Act
        var result = profile.AddMediaItem(mediaItem);

        // Assert
        result.IsSuccess.Should().BeTrue();
        profile.MediaItems.Should().HaveCount(1);
        profile.MediaItems.First().Type.Should().Be(MediaType.Image);
    }

    [Fact]
    public void AddPost_ShouldSucceed()
    {
        // Arrange
        var userId = new UserId(Guid.CreateVersion7());
        var profileResult = LecturerProfile.CreateProfile(userId);
        var profile = profileResult.Value;

        var post = Post.Create(
            "Getting Started with Microservices",
            "In this article, we'll explore the fundamentals of microservices architecture...",
            "https://example.com/post-thumbnail.jpg");

        // Act
        var result = profile.AddPost(post);

        // Assert
        result.IsSuccess.Should().BeTrue();
        profile.Posts.Should().HaveCount(1);
        profile.Posts.First().Title.Should().Be("Getting Started with Microservices");
    }

    [Fact]
    public void RemoveMediaItem_WithNonExistentItem_ShouldFail()
    {
        // Arrange
        var userId = new UserId(Guid.CreateVersion7());
        var profileResult = LecturerProfile.CreateProfile(userId);
        var profile = profileResult.Value;

        var nonExistentId = Guid.NewGuid();

        // Act
        var result = profile.RemoveMediaItem(nonExistentId);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("LecturerProfile.MediaItemNotFound");
    }

    [Fact]
    public void RemovePost_WithNonExistentPost_ShouldFail()
    {
        // Arrange
        var userId = new UserId(Guid.CreateVersion7());
        var profileResult = LecturerProfile.CreateProfile(userId);
        var profile = profileResult.Value;

        var nonExistentId = Guid.NewGuid();

        // Act
        var result = profile.RemovePost(nonExistentId);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("LecturerProfile.PostNotFound");
    }
}
