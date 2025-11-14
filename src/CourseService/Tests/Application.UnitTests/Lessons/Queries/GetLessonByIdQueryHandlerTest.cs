using Application.Lessons.Queries.GetById;
using Application.Abstractions.Data;
using Application.Courses.Queries.Dtos;
using Domain.Lessons;
using Domain.Lessons.Errors;
using Domain.Lessons.Primitives;
using Moq;
using FluentAssertions;
using Xunit;
using Moq.EntityFrameworkCore;

namespace Application.UnitTests.Lessons.Queries;

public class GetLessonByIdQueryHandlerTest
{
    private readonly Mock<IReadDbContext> _dbContextMock;
    private readonly GetLessonByIdQueryHandler _handler;
    private readonly Lesson _testLesson;

    public GetLessonByIdQueryHandlerTest()
    {
        _dbContextMock = new Mock<IReadDbContext>();
        _handler = new GetLessonByIdQueryHandler(_dbContextMock.Object);

        // Create a test lesson
        _testLesson = Lesson.CreateLesson(
            "Test Lesson",
            "Test Description",
            "https://test.com/video.mp4",
            "https://test.com/thumb.jpg",
            true,
            1,
            TimeSpan.FromMinutes(30)
        );
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenLessonDoesNotExist()
    {
        // Arrange
        var nonExistentLessonId = new LessonId(Guid.NewGuid());
        var query = new GetLessonByIdQuery(nonExistentLessonId);

        _dbContextMock.Setup(db => db.Lessons).ReturnsDbSet(new List<Lesson>());

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(LessonErrors.NotFound);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenLessonExists()
    {
        // Arrange
        var query = new GetLessonByIdQuery(_testLesson.Id);

        _dbContextMock.Setup(db => db.Lessons).ReturnsDbSet(new List<Lesson> { _testLesson });

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Id.Should().Be(_testLesson.Id);
    }

    [Fact]
    public async Task Handle_ShouldReturnLessonWithCorrectProperties()
    {
        // Arrange
        var expectedTitle = "Advanced Programming";
        var expectedDescription = "Learn advanced concepts";
        var expectedVideoUrl = "https://test.com/advanced.mp4";
        var expectedThumbnail = "https://test.com/advanced-thumb.jpg";
        var expectedIsPreview = true;
        var expectedOrder = 3;
        var expectedDuration = TimeSpan.FromHours(1.5);

        var lesson = Lesson.CreateLesson(
            expectedTitle,
            expectedDescription,
            expectedVideoUrl,
            expectedThumbnail,
            expectedIsPreview,
            expectedOrder,
            expectedDuration
        );

        var query = new GetLessonByIdQuery(lesson.Id);

        _dbContextMock.Setup(db => db.Lessons).ReturnsDbSet(new List<Lesson> { lesson });

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Title.Should().Be(expectedTitle);
        result.Value.Description.Should().Be(expectedDescription);
        result.Value.VideoUrl.Should().Be(expectedVideoUrl);
        result.Value.ThumbnailImage.Should().Be(expectedThumbnail);
        result.Value.IsPreview.Should().Be(expectedIsPreview);
        result.Value.Order.Should().Be(expectedOrder);
        result.Value.Duration.Should().Be(expectedDuration);
    }

    [Fact]
    public async Task Handle_ShouldReturnLessonWithNullOptionalFields()
    {
        // Arrange
        var lesson = Lesson.CreateLesson(
            "Basic Lesson",
            null, // Description is optional
            null, // VideoUrl is optional
            null, // ThumbnailImage is optional
            false,
            1,
            null  // Duration is optional
        );

        var query = new GetLessonByIdQuery(lesson.Id);

        _dbContextMock.Setup(db => db.Lessons).ReturnsDbSet(new List<Lesson> { lesson });

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Title.Should().Be("Basic Lesson");
        result.Value.Description.Should().BeNull();
        result.Value.VideoUrl.Should().BeNull();
        result.Value.ThumbnailImage.Should().BeNull();
        result.Value.Duration.Should().BeNull();
    }
}
