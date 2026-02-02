using Courses.Application.Abstractions.Data;
using Courses.Application.Abstractions.Storage;
using Courses.Application.Lessons.Queries.GetLessons;
using Courses.Application.Services.LinkProvider.Abstractions;
using Courses.Domain.Lessons;
using Courses.Domain.Lessons.Primitives;
using FluentAssertions;
using Moq;
using Moq.EntityFrameworkCore;
using Xunit;

namespace Application.UnitTests.Lessons.Queries;

public class GetLessonsQueryHandlerTest
{
    private readonly Mock<IReadDbContext> _dbContextMock;
    private readonly Mock<IStorageUrlResolver> _urlResolverMock;
    private readonly Mock<ILinkBuilderService> _linkBuilderMock;
    private readonly GetLessonsQueryHandler _handler;

    public GetLessonsQueryHandlerTest()
    {
        _dbContextMock = new Mock<IReadDbContext>();
        _urlResolverMock = new Mock<IStorageUrlResolver>();
        _linkBuilderMock = new Mock<ILinkBuilderService>();

        _urlResolverMock
            .Setup(r => r.Resolve(It.IsAny<StorageCategory>(), It.IsAny<string>()))
            .Returns(new ResolvedUrl("https://example.com/", StorageCategory.Public));

        _linkBuilderMock
            .Setup(l => l.BuildLinks(It.IsAny<LinkResourceKey>(), It.IsAny<object>()))
            .Returns(new List<LinkDto>().AsReadOnly());

        _handler = new GetLessonsQueryHandler(
            _dbContextMock.Object,
            _urlResolverMock.Object,
            _linkBuilderMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenLessonDoesNotExist()
    {
        // Arrange
        var nonExistentLessonId = new LessonId(Guid.NewGuid());
        var query = new GetLessonsQuery(new LessonFilter(Ids: [nonExistentLessonId.Value], IncludeDetails: true));

        _dbContextMock.Setup(db => db.Lessons).ReturnsDbSet(new List<Lesson>());
        _dbContextMock.Setup(db => db.Courses).ReturnsDbSet(new List<Domain.Courses.Course>());

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }
}
