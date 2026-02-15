using Courses.Application.Abstractions.Data;
using Courses.Application.Abstractions.Storage;
using Courses.Application.Features.Shared;
using Courses.Application.Features.StudentDashboard.GetEnrolledCourses;
using Courses.Application.Services.LinkProvider.Abstractions;
using Courses.Application.Services.LinkProvider.Abstractions.Links;
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
    : IQueryHandler<GetEnrolledCoursesQuery, GetEnrolledCoursesDto>
{
    private readonly ILinkProvider _linkProvider;
    private readonly IReadDbContext _readDbContext;
    private readonly IUserContext _userContext;
    private readonly IStorageUrlResolver _storageUrlResolver;

    public GetEnrolledCoursesQueryHandler(
        ILinkProvider linkProvider,
        IReadDbContext readDbContext,
        IUserContext userContext,
        IStorageUrlResolver storageUrlResolver)
    {
        _linkProvider = linkProvider;
        _readDbContext = readDbContext;
        _userContext = userContext;
        _storageUrlResolver = storageUrlResolver;
    }

    public async Task<Result<GetEnrolledCoursesDto>> Handle(
        GetEnrolledCoursesQuery request,
        CancellationToken cancellationToken = default)
    {
        if (!_userContext.IsAuthenticated || _userContext.Id is null)
        {
            return Result.Failure<GetEnrolledCoursesDto>(EnrollmentErrors.Unauthenticated);
        }

        var studentId = new UserId(_userContext.Id.Value);

        (List<EnrolledCourseRawData> rawData, int totalCount) =
            await FetchEnrolledCoursesDataAsync(studentId, request, cancellationToken);

        if (totalCount == 0)
        {
            return Result.Success(CreateCollectionResponse([], totalCount, request));
        }

        List<EnrolledCourseItemDto> items = MapToItemDtos(rawData);
        return Result.Success(CreateCollectionResponse(items, totalCount, request));
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

    private List<EnrolledCourseItemDto> MapToItemDtos(List<EnrolledCourseRawData> rawData)
    {
        return rawData.Select(item =>
        {
            Enrollment enrollment = item.Enrollment;
            Guid courseId = enrollment.CourseId.Value;
            double progress = CalculateProgress(enrollment.CompletedLessons.Count, item.TotalLessons);

            EnrolledCourseLinks links = BuildItemLinks(enrollment);
            EnrolledCourseItemData data = new(
                EnrollmentId: enrollment.Id.Value,
                CourseId: courseId,
                CourseTitle: item.CourseInfo?.Title.Value ?? string.Empty,
                CourseImageUrl: GetImageUrl(item.CourseInfo?.Images),
                CourseSlug: item.CourseInfo?.Slug.Value ?? string.Empty,
                LastAccessedAt: enrollment.LastAccessedAt,
                EnrolledAt: enrollment.EnrolledAt,
                Status: enrollment.Status.ToString(),
                ProgressPercentage: progress);

            return new EnrolledCourseItemDto(Data: data, Links: links);
        }).ToList();
    }

    private EnrolledCourseLinks BuildItemLinks(Enrollment enrollment)
    {
        Guid courseId = enrollment.CourseId.Value;
        LinkRecord viewCourse = _linkProvider.GetCoursePageLink(courseId);
        LinkRecord? continueLearning = enrollment.LastAccessedLessonId is not null
            ? _linkProvider.GetLessonPageLink(enrollment.LastAccessedLessonId!.Value)
            : _linkProvider.GetCoursePageLink(courseId);
        return new EnrolledCourseLinks(ViewCourse: viewCourse, ContinueLearning: continueLearning);
    }

    private GetEnrolledCoursesDto CreateCollectionResponse(
        List<EnrolledCourseItemDto> items,
        int total,
        GetEnrolledCoursesQuery request)
    {
        var collectionLinks = new GetEnrolledCoursesCollectionLinks(
            Self: _linkProvider.GetEnrolledCoursesLink(request.PageNumber, request.PageSize));

        return new GetEnrolledCoursesDto(
            Items: items,
            PageNumber: request.PageNumber,
            PageSize: request.PageSize,
            TotalItems: total,
            Links: collectionLinks);
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
