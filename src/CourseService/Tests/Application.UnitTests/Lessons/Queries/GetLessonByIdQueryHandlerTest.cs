using Courses.Application.Abstractions.Data;
using Courses.Application.Abstractions.Storage;
using Courses.Application.Lessons.Queries.GetById;
using Courses.Application.Services.LinkProvider.Abstractions;
using Courses.Domain.Lessons;
using Courses.Domain.Lessons.Errors;
using Courses.Domain.Lessons.Primitives;
using Courses.Domain.Modules.Primitives;
using FluentAssertions;
using Moq;
using Moq.EntityFrameworkCore;
using Xunit;

namespace Application.UnitTests.Lessons.Queries;

public class GetLessonByIdQueryHandlerTest
{
    private readonly Mock<IReadDbContext> _dbContextMock;
    private readonly Mock<IStorageUrlResolver> _urlResolverMock;
    private readonly Mock<ILinkBuilderService> _linkBuilderMock;
    private readonly GetLessonByIdQueryHandler _handler;

    public GetLessonByIdQueryHandlerTest()
    {
        _dbContextMock = new Mock<IReadDbContext>();
        _urlResolverMock = new Mock<IStorageUrlResolver>();
        _linkBuilderMock = new Mock<ILinkBuilderService>();

        _urlResolverMock
            .Setup(r => r.Resolve(It.IsAny<StorageCategory>(), It.IsAny<string>()))
            .Returns(new ResolvedUrl("https://example.com/", StorageCategory.Public));

        _linkBuilderMock
            .Setup(l => l.BuildLinks(It.IsAny<string>(), It.IsAny<object>()))
            .Returns(new List<LinkDto>().AsReadOnly());

        _handler = new GetLessonByIdQueryHandler(
            _urlResolverMock.Object,
            _dbContextMock.Object,
            _linkBuilderMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenLessonDoesNotExist()
    {
        // Arrange
        var moduleId = new ModuleId(Guid.NewGuid());
        var nonExistentLessonId = new LessonId(Guid.NewGuid());
        var query = new GetLessonByIdQuery(moduleId, nonExistentLessonId);

        _dbContextMock.Setup(db => db.Lessons).ReturnsDbSet(new List<Lesson>());

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(LessonErrors.NotFound);
    }

    // Further tests (Handle_ShouldReturnSuccess_WhenLessonExists, etc.) require setting up
    // Lesson and Course in DbSets using the current Domain API (Lesson.Create, Course.CreateCourse)
    // and asserting on LessonDetailsPageDto (LessonId, Title, Description, VideoUrl, ThumbnailUrl, etc.).
}
