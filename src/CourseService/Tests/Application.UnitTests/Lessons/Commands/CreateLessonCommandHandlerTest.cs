using Application.Lessons.Commands.CreateLesson;
using Application.Abstractions.Data;
using Domain.Courses;
using Domain.Courses.Primitives;
using Domain.Courses.Errors;
using Domain.Lessons;
using Moq;
using FluentAssertions;
using Xunit;
using Moq.EntityFrameworkCore;
using SharedKernel;

namespace Application.UnitTests.Lessons.Commands;

public class CreateLessonCommandHandlerTest
{
    private readonly Mock<IWriteDbContext> _dbContextMock;
    private readonly CreateLessonCommandHandler _handler;
    private readonly Course _testCourse;

    public CreateLessonCommandHandlerTest()
    {
        _dbContextMock = new Mock<IWriteDbContext>();
        _handler = new CreateLessonCommandHandler(_dbContextMock.Object);
        
        // Create a test course
        _testCourse = Course.CreateCourse(
            "Test Course",
            "Test Description",
            "https://test.com/image.jpg",
            "instructor-123",
            new Money(100, "ILS")
        );
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenCourseDoesNotExist()
    {
        // Arrange
        var dto = new CreateLessonDto(
            "Test Lesson",
            "Lesson Description",
            "https://test.com/video.mp4",
            "https://test.com/thumb.jpg",
            false,
            1,
            TimeSpan.FromMinutes(30),
            Guid.NewGuid() // Non-existent course ID
        );
        var command = new CreateLessonCommand(dto);

        _dbContextMock.Setup(db => db.Courses).ReturnsDbSet(new List<Course>());

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(CourseErrors.NotFound);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenCourseExists()
    {
        // Arrange
        var dto = new CreateLessonDto(
            "Test Lesson",
            "Lesson Description",
            "https://test.com/video.mp4",
            "https://test.com/thumb.jpg",
            true,
            2,
            TimeSpan.FromMinutes(45),
            _testCourse.Id.Value
        );
        var command = new CreateLessonCommand(dto);

        _dbContextMock.Setup(db => db.Courses).ReturnsDbSet(new List<Course> { _testCourse });
        _dbContextMock.Setup(db => db.Lessons).ReturnsDbSet(new List<Lesson>());
        _dbContextMock.Setup(db => db.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Value.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Handle_ShouldCreateLessonWithCorrectProperties()
    {
        // Arrange
        var expectedTitle = "Advanced Programming";
        var expectedDescription = "Learn advanced concepts";
        var expectedVideoUrl = "https://test.com/advanced.mp4";
        var expectedThumbnail = "https://test.com/advanced-thumb.jpg";
        var expectedIsPreview = true;
        var expectedOrder = 3;
        var expectedDuration = TimeSpan.FromHours(1.5);

        var dto = new CreateLessonDto(
            expectedTitle,
            expectedDescription,
            expectedVideoUrl,
            expectedThumbnail,
            expectedIsPreview,
            expectedOrder,
            expectedDuration,
            _testCourse.Id.Value
        );
        var command = new CreateLessonCommand(dto);

        Lesson? capturedLesson = null;
        _dbContextMock.Setup(db => db.Courses).ReturnsDbSet(new List<Course> { _testCourse });
        _dbContextMock.Setup(db => db.Lessons).ReturnsDbSet(new List<Lesson>());
        _dbContextMock.Setup(db => db.Lessons.AddAsync(It.IsAny<Lesson>(), It.IsAny<CancellationToken>()))
            .Callback<Lesson, CancellationToken>((lesson, _) => capturedLesson = lesson)
            .Returns(default(ValueTask<Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<Lesson>>)!);
        _dbContextMock.Setup(db => db.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        capturedLesson.Should().NotBeNull();
        capturedLesson!.Title.Should().Be(expectedTitle);
        capturedLesson.Description.Should().Be(expectedDescription);
        capturedLesson.VideoUrl.Should().Be(expectedVideoUrl);
        capturedLesson.ThumbnailImage.Should().Be(expectedThumbnail);
        capturedLesson.IsPreview.Should().Be(expectedIsPreview);
        capturedLesson.Order.Should().Be(expectedOrder);
        capturedLesson.Duration.Should().Be(expectedDuration);
        capturedLesson.CourseId.Should().Be(_testCourse.Id);
    }

    [Fact]
    public async Task Handle_ShouldCreateLessonWithNullOptionalFields()
    {
        // Arrange
        var dto = new CreateLessonDto(
            "Basic Lesson",
            null, // Description is optional
            null, // VideoUrl is optional
            null, // ThumbnailImage is optional
            false,
            1,
            null, // Duration is optional
            _testCourse.Id.Value
        );
        var command = new CreateLessonCommand(dto);

        _dbContextMock.Setup(db => db.Courses).ReturnsDbSet(new List<Course> { _testCourse });
        _dbContextMock.Setup(db => db.Lessons).ReturnsDbSet(new List<Lesson>());
        _dbContextMock.Setup(db => db.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
    }
}
