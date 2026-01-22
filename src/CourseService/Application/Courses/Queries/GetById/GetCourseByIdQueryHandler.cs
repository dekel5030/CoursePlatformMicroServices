using System.Data;
using System.Text.Json;
using Courses.Application.Abstractions.Data;
using Courses.Application.Abstractions.Storage;
using Courses.Application.Categories.Dtos;
using Courses.Application.Courses.Dtos;
using Courses.Application.Lessons.Dtos;
using Courses.Application.Modules.Dtos;
using Courses.Application.Shared.Dtos;
using SharedDtos = Courses.Application.Shared.Dtos;
using Courses.Domain.Courses.Errors;
using Courses.Domain.Courses.Primitives;
using Courses.Domain.Lessons.Primitives;
using Courses.Domain.Module.Primitives;
using Courses.Domain.Shared.Primitives;
using Dapper;
using Kernel;
using Kernel.Messaging.Abstractions;
using Courses.Domain.Categories;

namespace Courses.Application.Courses.Queries.GetById;

internal sealed class GetCourseByIdQueryHandler : IQueryHandler<GetCourseByIdQuery, CoursePageDto>
{
    private readonly ISqlConnectionFactory _connectionFactory;
    private readonly IStorageUrlResolver _urlResolver;

    public GetCourseByIdQueryHandler(
        ISqlConnectionFactory connectionFactory,
        IStorageUrlResolver urlResolver)
    {
        _connectionFactory = connectionFactory;
        _urlResolver = urlResolver;
    }

    public async Task<Result<CoursePageDto>> Handle(
        GetCourseByIdQuery request,
        CancellationToken cancellationToken = default)
    {
        using IDbConnection connection = _connectionFactory.CreateConnection();

        const string courseSql = @"
            SELECT 
                c.id,
                c.title,
                c.description,
                c.status,
                c.price_amount AS PriceAmount,
                c.price_currency AS PriceCurrency,
                c.enrollment_count AS EnrollmentCount,
                c.lesson_count AS LessonCount,
                c.total_duration AS TotalDuration,
                c.updated_at_utc AS UpdatedAtUtc,
                c.instructor_id AS InstructorId,
                c.category_id AS CategoryId,
                c.course_images AS CourseImages,
                c.course_tags AS CourseTags,
                u.first_name AS FirstName,
                u.last_name AS LastName,
                u.avatar_url AS AvatarUrl,
                cat.name AS CategoryName,
                cat.slug AS CategorySlug
            FROM courses c
            LEFT JOIN users u ON c.instructor_id = u.id
            LEFT JOIN categories cat ON c.category_id = cat.id
            WHERE c.id = @CourseId";

        SharedDtos.CourseRow? courseRow = await connection.QueryFirstOrDefaultAsync<SharedDtos.CourseRow>(
            new CommandDefinition(courseSql, new { CourseId = request.Id.Value }, cancellationToken: cancellationToken));

        if (courseRow is null)
        {
            return Result.Failure<CoursePageDto>(CourseErrors.NotFound);
        }

        const string modulesSql = @"
            SELECT 
                m.id,
                m.title,
                m.index,
                m.course_id AS CourseId
            FROM modules m
            WHERE m.course_id = @CourseId
            ORDER BY m.index";

        IEnumerable<SharedDtos.ModuleRow> modules = await connection.QueryAsync<SharedDtos.ModuleRow>(
            new CommandDefinition(modulesSql, new { CourseId = request.Id.Value }, cancellationToken: cancellationToken));

        var moduleIds = modules.Select(m => m.Id).ToList();

            const string lessonsSql = @"
            SELECT 
                l.id,
                l.module_id AS ModuleId,
                l.title,
                l.description,
                l.index,
                l.duration,
                l.thumbnail_image_url AS ThumbnailImageUrl,
                l.video_url AS VideoUrl,
                l.access
            FROM lessons l
            WHERE l.module_id = ANY(@ModuleIds)
            ORDER BY l.module_id, l.index";

        IEnumerable<SharedDtos.LessonRow> lessons = moduleIds.Count > 0
            ? await connection.QueryAsync<SharedDtos.LessonRow>(
                new CommandDefinition(lessonsSql, new { ModuleIds = moduleIds.ToArray() }, cancellationToken: cancellationToken))
            : Enumerable.Empty<SharedDtos.LessonRow>();

        var lessonsByModuleId = lessons.GroupBy(l => l.ModuleId).ToDictionary(g => g.Key, g => g.ToList());

        var moduleDtos = modules.Select(m =>
        {
            List<SharedDtos.LessonRow> moduleLessons = lessonsByModuleId.GetValueOrDefault(m.Id, new List<SharedDtos.LessonRow>());
            var lessonDtos = moduleLessons
                .Select(l => new LessonSummaryDto(
                    new CourseId(m.CourseId),
                    new ModuleId(m.Id),
                    new LessonId(l.Id),
                    new Title(l.Title),
                    l.Index,
                    l.Duration,
                    l.ThumbnailImageUrl,
                    Enum.Parse<LessonAccess>(l.Access, ignoreCase: true)))
                .ToList();

            var totalDuration = TimeSpan.FromTicks(moduleLessons.Sum(l => l.Duration.Ticks));

            return new ModuleDetailsDto(
                new ModuleId(m.Id),
                new Title(m.Title),
                m.Index,
                moduleLessons.Count,
                totalDuration,
                lessonDtos);
        }).ToList();

        List<string> imageUrls = courseRow.CourseImages != null
            ? JsonSerializer.Deserialize<List<SharedDtos.ImageUrlData>>(courseRow.CourseImages)
                ?.Select(i => _urlResolver.Resolve(StorageCategory.Public, i.Path).Value)
                .ToList() ?? new List<string>()
            : new List<string>();

        List<TagDto> tagDtos = courseRow.CourseTags != null
            ? JsonSerializer.Deserialize<List<SharedDtos.TagData>>(courseRow.CourseTags)
                ?.Select(t => new TagDto(t.Value))
                .ToList() ?? new List<TagDto>()
            : new List<TagDto>();

        var instructorDto = new InstructorDto(
            new UserId(courseRow.InstructorId),
            courseRow.FirstName != null && courseRow.LastName != null
                ? $"{courseRow.FirstName} {courseRow.LastName}"
                : "Unknown",
            courseRow.AvatarUrl);

        var categoryDto = new CategoryDto(
            new CategoryId(courseRow.CategoryId),
            courseRow.CategoryName ?? "Unknown",
            courseRow.CategorySlug != null
                ? new Slug(courseRow.CategorySlug)
                : new Slug("unknown"));

        var coursePageDto = new CoursePageDto(
            new CourseId(courseRow.Id),
            new Title(courseRow.Title),
            new Description(courseRow.Description),
            instructorDto,
            Enum.Parse<CourseStatus>(courseRow.Status, ignoreCase: true),
            new Money(courseRow.PriceAmount, courseRow.PriceCurrency),
            courseRow.EnrollmentCount,
            courseRow.LessonCount,
            courseRow.TotalDuration,
            courseRow.UpdatedAtUtc,
            imageUrls,
            tagDtos,
            categoryDto,
            moduleDtos);

        return Result.Success(coursePageDto);
    }
}
