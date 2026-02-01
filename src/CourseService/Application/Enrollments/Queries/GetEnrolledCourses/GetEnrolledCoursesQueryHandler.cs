using Courses.Application.Abstractions.Data;
using Courses.Application.Enrollments.Dtos;
using Courses.Application.Services.LinkProvider;
using Courses.Application.Services.LinkProvider.Abstractions;
using Courses.Domain.Courses;
using Courses.Domain.Courses.Primitives;
using Courses.Domain.Enrollments;
using Courses.Domain.Enrollments.Errors;
using Courses.Domain.Enrollments.Primitives;
using Kernel;
using Kernel.Auth.Abstractions;
using Kernel.Messaging.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Courses.Application.Enrollments.Queries.GetEnrolledCourses;

internal sealed class GetEnrolledCoursesQueryHandler : IQueryHandler<GetEnrolledCoursesQuery, EnrolledCourseCollectionDto>
{
    private readonly ILinkBuilderService _linkBuilder;
    private readonly IReadDbContext _readDbContext;
    private readonly IUserContext _userContext;

    public GetEnrolledCoursesQueryHandler(
        ILinkBuilderService linkBuilder,
        IReadDbContext readDbContext,
        IUserContext userContext)
    {
        _linkBuilder = linkBuilder;
        _readDbContext = readDbContext;
        _userContext = userContext;
    }

    public async Task<Result<EnrolledCourseCollectionDto>> Handle(
        GetEnrolledCoursesQuery request,
        CancellationToken cancellationToken = default)
    {
        if (_userContext.Id is null || !_userContext.IsAuthenticated)
        {
            return Result.Failure<EnrolledCourseCollectionDto>(EnrollmentErrors.Unauthenticated);
        }

        var studentId = new UserId(_userContext.Id.Value);

        IQueryable<Enrollment> query = _readDbContext.Enrollments
            .Where(e => e.StudentId == studentId && e.Status == EnrollmentStatus.Active);

        int totalCount = await query.CountAsync(cancellationToken);

        List<Enrollment> items = await query
            .OrderByDescending(e => e.LastAccessedAt ?? e.EnrolledAt)
            .ThenByDescending(e => e.EnrolledAt)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        if (items.Count == 0)
        {
            var emptyCollection = new EnrolledCourseCollectionDto
            {
                Items = Array.Empty<EnrolledCourseDto>(),
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalItems = 0,
                Links = _linkBuilder.BuildLinks(
                    LinkResourceKey.EnrolledCourseCollection,
                    new EnrolledCourseCollectionContext(request.PageNumber, request.PageSize, 0))
            };
            return Result.Success(emptyCollection);
        }

        var courseIds = items.Select(e => e.CourseId).Distinct().ToList();
        List<Course> courses = await _readDbContext.Courses
            .Where(c => courseIds.Contains(c.Id))
            .ToListAsync(cancellationToken);
        var courseMap = courses.ToDictionary(c => c.Id, c => c);

        Dictionary<CourseId, int> lessonCountByCourse = await _readDbContext.Lessons
            .Where(l => courseIds.Contains(l.CourseId))
            .GroupBy(l => l.CourseId)
            .ToDictionaryAsync(g => g.Key, g => g.Count(), cancellationToken);

        var dtos = new List<EnrolledCourseDto>();
        foreach (Enrollment enrollment in items)
        {
            Course? course = courseMap.GetValueOrDefault(enrollment.CourseId);
            int totalLessons = lessonCountByCourse.GetValueOrDefault(enrollment.CourseId, 0);
            double progressPercentage = totalLessons > 0
                ? enrollment.CompletedLessons.Count * 100.0 / totalLessons
                : 0;

            var linkContext = new EnrolledCourseContext(enrollment.CourseId, enrollment.LastAccessedLessonId);
            IReadOnlyList<LinkDto> links = _linkBuilder.BuildLinks(LinkResourceKey.EnrolledCourse, linkContext);

            dtos.Add(new EnrolledCourseDto
            {
                EnrollmentId = enrollment.Id.Value,
                CourseId = enrollment.CourseId.Value,
                CourseTitle = course?.Title.Value ?? string.Empty,
                CourseSlug = course?.Slug.Value ?? string.Empty,
                ProgressPercentage = progressPercentage,
                LastAccessedAt = enrollment.LastAccessedAt,
                EnrolledAt = enrollment.EnrolledAt,
                Status = enrollment.Status.ToString(),
                Links = links
            });
        }

        var collectionContext = new EnrolledCourseCollectionContext(
            request.PageNumber,
            request.PageSize,
            totalCount);
        IReadOnlyList<LinkDto> collectionLinks = _linkBuilder.BuildLinks(
            LinkResourceKey.EnrolledCourseCollection,
            collectionContext);

        var result = new EnrolledCourseCollectionDto
        {
            Items = dtos,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalItems = totalCount,
            Links = collectionLinks
        };

        return Result.Success(result);
    }
}
