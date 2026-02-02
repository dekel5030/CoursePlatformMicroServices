using Courses.Application.Abstractions.Data;
using Courses.Application.Abstractions.Storage;
using Courses.Application.Courses.Dtos;
using Courses.Application.Services.LinkProvider;
using Courses.Application.Services.LinkProvider.Abstractions;
using Courses.Domain.Courses;
using Courses.Domain.Courses.Errors;
using Kernel;
using Kernel.Messaging.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Courses.Application.Courses.Queries.GetById;

internal sealed class GetCourseByIdQueryHandler : IQueryHandler<GetCourseByIdQuery, CourseDto>
{
    private readonly IReadDbContext _readDbContext;
    private readonly IStorageUrlResolver _urlResolver;
    private readonly ILinkBuilderService _linkBuilder;

    public GetCourseByIdQueryHandler(
        IReadDbContext readDbContext,
        IStorageUrlResolver urlResolver,
        ILinkBuilderService linkBuilder)
    {
        _readDbContext = readDbContext;
        _urlResolver = urlResolver;
        _linkBuilder = linkBuilder;
    }

    public async Task<Result<CourseDto>> Handle(
        GetCourseByIdQuery request,
        CancellationToken cancellationToken = default)
    {
        Course? course = await _readDbContext.Courses
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

        if (course is null)
        {
            return Result.Failure<CourseDto>(CourseErrors.NotFound);
        }

        int enrollmentCount = await _readDbContext.Enrollments
            .CountAsync(e => e.CourseId == request.Id, cancellationToken);

        var courseContext = new CourseContext(course.Id, course.InstructorId, course.Status);
        var resolvedImageUrls = course.Images
            .Select(img => _urlResolver.Resolve(StorageCategory.Public, img.Path).Value)
            .ToList();

        var courseDto = new CourseDto
        {
            Id = course.Id.Value,
            Title = course.Title.Value,
            Description = course.Description.Value,
            InstructorId = course.InstructorId.Value,
            CategoryId = course.CategoryId.Value,
            Status = course.Status,
            Price = course.Price,
            EnrollmentCount = enrollmentCount,
            LessonsCount = 0,
            TotalDuration = TimeSpan.Zero,
            UpdatedAtUtc = course.UpdatedAtUtc,
            ImageUrls = resolvedImageUrls.AsReadOnly(),
            Tags = course.Tags.Select(t => t.Value).ToList().AsReadOnly(),
            ModuleIds = [],
            Links = _linkBuilder.BuildLinks(LinkResourceKey.Course, courseContext).ToList()
        };

        return Result.Success(courseDto);
    }
}
