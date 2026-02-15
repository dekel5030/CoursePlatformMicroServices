using Courses.Application.Abstractions.Data;
using Courses.Application.Abstractions.Storage;
using Courses.Application.Enrollments.Dtos;
using Courses.Application.Features.Shared;
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

namespace Courses.Application.Features.StudentDashboard.GetEnrolledCourses;

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
        if (!_userContext.IsAuthenticated || _userContext.Id is null)
        {
            return Result.Failure<EnrolledCourseCollectionDto>(EnrollmentErrors.Unauthenticated);
        }

        var studentId = new UserId(_userContext.Id.Value);

        (List<EnrolledCourseRawData>? rawData, int totalCount) = 
            await FetchEnrolledCoursesDataAsync(studentId, request, cancellationToken);

        if (totalCount == 0)
        {
            return Result.Success(CreateCollectionResponse(new(), totalCount, request));
        }

        List<EnrolledCourseWithAnalyticsDto> dtos = MapToEnrolledCourseDtos(rawData);

        return Result.Success(CreateCollectionResponse(dtos, totalCount, request));
    }

    private async Task<(List<EnrolledCourseRawData> Items, int TotalCount)> FetchEnrolledCoursesDataAsync(
        UserId studentId,
        GetEnrolledCoursesQuery request,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Enrollment> query = _readDbContext.Enrollments
            .Where(enrollment => 
                enrollment.StudentId == studentId && 
                enrollment.Status == EnrollmentStatus.Active);

        int totalCount = await query.CountAsync(cancellationToken);

        List<EnrolledCourseRawData> items = await query
            .OrderByDescending(enrollment => enrollment.LastAccessedAt ?? enrollment.EnrolledAt)
            .ThenByDescending(enrollment => enrollment.EnrolledAt)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(enrollment => new EnrolledCourseRawData
            {
                Enrollment = enrollment,
                CourseInfo = _readDbContext.Courses
                    .Where(course => course.Id == enrollment.CourseId)
                    .Select(course => new CourseInfo
                    {
                        Title = course.Title,
                        Slug = course.Slug,
                        Images = course.Images
                    })
                    .FirstOrDefault(),
                TotalLessons = _readDbContext.Lessons.Count(lesson => lesson.CourseId == enrollment.CourseId)
            })
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    private List<EnrolledCourseWithAnalyticsDto> MapToEnrolledCourseDtos(List<EnrolledCourseRawData> rawData)
    {
        return rawData.Select(item =>
        {
            Enrollment enrollment = item.Enrollment;

            double progress = CalculateProgress(enrollment.CompletedLessons.Count, item.TotalLessons);

            var enrolledCourseDto = new EnrolledCourseDto
            {
                EnrollmentId = enrollment.Id.Value,
                CourseId = enrollment.CourseId.Value,
                CourseTitle = item.CourseInfo?.Title.Value ?? string.Empty,
                CourseImageUrl = GetImageUrl(item.CourseInfo?.Images),
                CourseSlug = item.CourseInfo?.Slug.Value ?? string.Empty,
                LastAccessedAt = enrollment.LastAccessedAt,
                EnrolledAt = enrollment.EnrolledAt,
                Status = enrollment.Status.ToString(),
                Links = BuildItemLinks(enrollment)
            };

            return new EnrolledCourseWithAnalyticsDto(enrolledCourseDto, new EnrolledCourseAnalyticsDto(progress));
        }).ToList();
    }

    private EnrolledCourseCollectionDto CreateCollectionResponse(
        List<EnrolledCourseWithAnalyticsDto> items, int total, GetEnrolledCoursesQuery request)
    {
        var context = new EnrolledCourseCollectionContext(request.PageNumber, request.PageSize, total);

        return new EnrolledCourseCollectionDto
        {
            Items = items,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalItems = total,
            Links = _linkBuilder.BuildLinks(LinkResourceKey.EnrolledCourseCollection, context)
        };
    }

    private static double CalculateProgress(int completedCount, int totalCount)
    {
        if (totalCount <= 0)
        {
            return 0;
        }

        return (double)completedCount * 100 / totalCount;
    }

    private string? GetImageUrl(IReadOnlyCollection<ImageUrl>? images)
    {
        return images != null
            ? CourseSummaryHelpers.GetFirstImagePublicUrl(images, _storageUrlResolver)
            : null;
    }

    private IReadOnlyList<LinkDto> BuildItemLinks(Enrollment enrollment)
    {
        var context = new EnrolledCourseContext(enrollment.CourseId, enrollment.LastAccessedLessonId);
        return _linkBuilder.BuildLinks(LinkResourceKey.EnrolledCourse, context);
    }


    private sealed class EnrolledCourseRawData
    {
        public required Enrollment Enrollment { get; init; }
        public CourseInfo? CourseInfo { get; init; }
        public int TotalLessons { get; init; }
    }

    private sealed class CourseInfo
    {
        public required Title Title { get; init; }
        public required Slug Slug { get; init; }
        public required IReadOnlyCollection<ImageUrl> Images { get; init; }
    }
}
