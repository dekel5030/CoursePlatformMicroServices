using System.Data;
using System.Text.Json;
using Courses.Application.Abstractions.Data;
using Courses.Application.Abstractions.Storage;
using Courses.Application.Courses.Dtos;
using Courses.Application.Shared.Dtos;
using Courses.Domain.Courses.Primitives;
using Courses.Domain.Shared.Primitives;
using Dapper;
using Kernel;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Courses.Queries.GetCourses;

internal sealed class GetCoursesQueryHandler : IQueryHandler<GetCoursesQuery, CourseCollectionDto>
{
    private readonly ISqlConnectionFactory _connectionFactory;
#pragma warning disable S4487 // Unread "private" fields should be removed
    private readonly IStorageUrlResolver _urlResolver;
#pragma warning restore S4487 // Unread "private" fields should be removed

    public GetCoursesQueryHandler(
        ISqlConnectionFactory connectionFactory,
        IStorageUrlResolver urlResolver)
    {
        _connectionFactory = connectionFactory;
        _urlResolver = urlResolver;
    }

    public async Task<Result<CourseCollectionDto>> Handle(
        GetCoursesQuery request,
        CancellationToken cancellationToken = default)
    {
        int pageNumber = Math.Max(1, request.PagedQuery.PageNumber ?? 1);
        int pageSize = Math.Clamp(request.PagedQuery.PageSize ?? 10, 1, 100);

        using IDbConnection connection = _connectionFactory.CreateConnection();

        // Count total items
        const string countSql = "SELECT COUNT(*) FROM courses";
        int totalItems = await connection.QuerySingleAsync<int>(
            new CommandDefinition(countSql, cancellationToken: cancellationToken));

        // Get paginated courses with instructor info in a single query
        const string coursesSql = @"
            SELECT 
                c.id,
                c.title,
                c.status,
                c.price_amount AS PriceAmount,
                c.price_currency AS PriceCurrency,
                c.enrollment_count AS EnrollmentCount,
                c.lesson_count AS LessonCount,
                c.updated_at_utc AS UpdatedAtUtc,
                c.instructor_id AS InstructorId,
                c.course_images AS CourseImages,
                u.first_name AS FirstName,
                u.last_name AS LastName,
                u.avatar_url AS AvatarUrl
            FROM courses c
            LEFT JOIN users u ON c.instructor_id = u.id
            ORDER BY c.updated_at_utc DESC
            LIMIT @PageSize OFFSET @Offset";

        int offset = (pageNumber - 1) * pageSize;

        IEnumerable<CourseSummaryRow> coursesData = await connection.QueryAsync<CourseSummaryRow>(
            new CommandDefinition(
                coursesSql,
                new { PageSize = pageSize, Offset = offset },
                cancellationToken: cancellationToken));

        var coursesList = coursesData.ToList();

        if (coursesList.Count == 0)
        {
            return Result.Success(new CourseCollectionDto(
                new List<CourseSummaryDto>(),
                pageNumber,
                pageSize,
                totalItems));
        }

        var courseIds = coursesList.Select(c => c.Id).ToList();

        const string lessonCountsSql = @"
            SELECT 
                m.course_id AS CourseId,
                COUNT(l.id) AS LessonCount
            FROM modules m
            LEFT JOIN lessons l ON m.id = l.module_id
            WHERE m.course_id = ANY(@CourseIds)
            GROUP BY m.course_id";

        IEnumerable<LessonCountRow> lessonCounts = await connection.QueryAsync<LessonCountRow>(
            new CommandDefinition(lessonCountsSql, new { CourseIds = courseIds.ToArray() }, cancellationToken: cancellationToken));

        var lessonCountsByCourse = lessonCounts.ToDictionary(lc => lc.CourseId, lc => lc.LessonCount);

        var courses = coursesList.Select(course =>
        {
            int lessonCount = lessonCountsByCourse.GetValueOrDefault(course.Id, course.LessonCount);

            string? thumbnailUrl = course.CourseImages != null
                ? JsonSerializer.Deserialize<List<ImageUrlData>>(course.CourseImages)
                    ?.FirstOrDefault()?.Path
                : null;

            return new CourseSummaryDto(
                new CourseId(course.Id),
                new Title(course.Title),
                new InstructorDto(
                    new UserId(course.InstructorId),
                    course.FirstName != null && course.LastName != null
                        ? $"{course.FirstName} {course.LastName}"
                        : "Unknown",
                    course.AvatarUrl),
                Enum.Parse<CourseStatus>(course.Status, ignoreCase: true),
                new Money(course.PriceAmount, course.PriceCurrency),
                thumbnailUrl,
                lessonCount,
                course.EnrollmentCount,
                course.UpdatedAtUtc);
        }).ToList();

        var response = new CourseCollectionDto(
            courses,
            pageNumber,
            pageSize,
            totalItems);

        return Result.Success(response);
    }

    private sealed record CourseSummaryRow(
        Guid Id,
        string Title,
        string Status,
        decimal PriceAmount,
        string PriceCurrency,
        int EnrollmentCount,
        int LessonCount,
        DateTimeOffset UpdatedAtUtc,
        Guid InstructorId,
        string? CourseImages,
        string? FirstName,
        string? LastName,
        string? AvatarUrl
    );

    private sealed record LessonCountRow(
        Guid CourseId,
        int LessonCount
    );
}
