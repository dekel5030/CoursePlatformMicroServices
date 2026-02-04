using Courses.Application.Abstractions.Data;
using Courses.Application.Abstractions.Storage;
using Courses.Application.Enrollments.Dtos;
using Courses.Application.Services.LinkProvider;
using Courses.Application.Services.LinkProvider.Abstractions;
using Courses.Domain.Courses.Primitives;
using Courses.Domain.Enrollments;
using Courses.Domain.Enrollments.Errors;
using Courses.Domain.Enrollments.Primitives;
using Courses.Domain.Shared.Primitives;
using Kernel;
using Kernel.Auth.Abstractions;
using Kernel.Messaging.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Courses.Application.Enrollments.Queries.GetEnrolledCourses;

internal sealed class GetEnrolledCoursesQueryHandler
    : IQueryHandler<GetEnrolledCoursesQuery, EnrolledCourseCollectionDto>
{
    private readonly ILinkBuilderService _linkBuilder;
    private readonly IReadDbContext _readDbContext;
    private readonly IUserContext _userContext;
    private readonly IStorageUrlResolver _storageUrlResolver;

    public GetEnrolledCoursesQueryHandler(
        ILinkBuilderService linkBuilder,
        IReadDbContext readDbContext,
        IUserContext userContext,
        IStorageUrlResolver storageUrlResolver)
    {
        _linkBuilder = linkBuilder;
        _readDbContext = readDbContext;
        _userContext = userContext;
        _storageUrlResolver = storageUrlResolver;
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

        if (totalCount == 0)
        {
            return CreateEmptyResult(request);
        }

        var itemsWithData = await query
            .OrderByDescending(e => e.LastAccessedAt ?? e.EnrolledAt)
            .ThenByDescending(e => e.EnrolledAt)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(enrollment => new
            {
                Enrollment = enrollment,
                CourseInfo = _readDbContext.Courses
                    .Where(course => course.Id == enrollment.CourseId)
                    .Select(course => new { course.Title, course.Slug, course.Images })
                    .FirstOrDefault(),
                TotalLessons = _readDbContext.Lessons.Count(l => l.CourseId == enrollment.CourseId)
            })
            .ToListAsync(cancellationToken);

        var dtos = itemsWithData.Select(item =>
        {
            Enrollment enrollment = item.Enrollment;

            double progress = item.TotalLessons > 0
                ? (double)enrollment.CompletedLessons.Count * 100 / item.TotalLessons
                : 0;

            var linkContext = new EnrolledCourseContext(enrollment.CourseId, enrollment.LastAccessedLessonId);
            ImageUrl? courseImage = item.CourseInfo?.Images.FirstOrDefault();
            string? courseImageUrl = courseImage != null
                ? _storageUrlResolver.Resolve(StorageCategory.Public, courseImage.Path).Value
                : null;

            var enrolledCourseDto = new EnrolledCourseDto
            {
                EnrollmentId = enrollment.Id.Value,
                CourseId = enrollment.CourseId.Value,
                CourseTitle = item.CourseInfo?.Title.Value ?? string.Empty,
                CourseImageUrl = courseImageUrl,
                CourseSlug = item.CourseInfo?.Slug.Value ?? string.Empty,
                LastAccessedAt = enrollment.LastAccessedAt,
                EnrolledAt = enrollment.EnrolledAt,
                Status = enrollment.Status.ToString(),
                Links = _linkBuilder.BuildLinks(LinkResourceKey.EnrolledCourse, linkContext)
            };

            var analyticsDto = new EnrolledCourseAnalyticsDto(progress);

            return new EnrolledCourseWithAnalyticsDto(enrolledCourseDto, analyticsDto);
        }).ToList();

        return Result.Success(new EnrolledCourseCollectionDto
        {
            Items = dtos,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalItems = totalCount,
            Links = _linkBuilder.BuildLinks(
                LinkResourceKey.EnrolledCourseCollection,
                new EnrolledCourseCollectionContext(request.PageNumber, request.PageSize, totalCount))
        });
    }

    private Result<EnrolledCourseCollectionDto> CreateEmptyResult(GetEnrolledCoursesQuery request)
    {
        return Result.Success(new EnrolledCourseCollectionDto
        {
            Items = new List<EnrolledCourseWithAnalyticsDto>(),
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalItems = 0,
            Links = _linkBuilder.BuildLinks(
                LinkResourceKey.EnrolledCourseCollection,
                new EnrolledCourseCollectionContext(request.PageNumber, request.PageSize, 0))
        });
    }
}
