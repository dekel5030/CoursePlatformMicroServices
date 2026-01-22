using System.Data;
using Courses.Application.Abstractions.Data;
using Courses.Application.Lessons.Dtos;
using Courses.Application.Lessons.Primitives;
using Courses.Application.Modules.Dtos;
using Courses.Application.Shared.Dtos;
using Courses.Domain.Courses.Primitives;
using Courses.Domain.Lessons.Primitives;
using Courses.Domain.Module.Primitives;
using Courses.Domain.Shared.Primitives;
using Dapper;
using Kernel;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Modules.Queries.GetByCourseId;

internal sealed class GetModulesByCourseIdQueryHandler : IQueryHandler<GetModulesByCourseIdQuery, IReadOnlyList<ModuleDetailsDto>>
{
    private readonly ISqlConnectionFactory _connectionFactory;

    public GetModulesByCourseIdQueryHandler(ISqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<Result<IReadOnlyList<ModuleDetailsDto>>> Handle(
        GetModulesByCourseIdQuery request,
        CancellationToken cancellationToken = default)
    {
        using IDbConnection connection = _connectionFactory.CreateConnection();

        // Get course status in a single query with modules
        const string courseStatusSql = @"
            SELECT status, lesson_count
            FROM courses
            WHERE id = @CourseId";

        var courseStatusRow = await connection.QueryFirstOrDefaultAsync<CourseStatusRow>(
            new CommandDefinition(courseStatusSql, new { CourseId = request.CourseId.Value }, cancellationToken: cancellationToken));

        if (courseStatusRow is null)
        {
            return Result.Failure<IReadOnlyList<ModuleDetailsDto>>(
                Error.NotFound("Course.NotFound", "The specified course was not found."));
        }

        bool isPublished = Enum.Parse<CourseStatus>(courseStatusRow.Status, ignoreCase: true) == CourseStatus.Published;

        // Get all modules for the course
        const string modulesSql = @"
            SELECT 
                id,
                title,
                index,
                course_id AS CourseId
            FROM modules
            WHERE course_id = @CourseId
            ORDER BY index";

        IEnumerable<Courses.Application.Shared.Dtos.ModuleRow> modules = await connection.QueryAsync<Courses.Application.Shared.Dtos.ModuleRow>(
            new CommandDefinition(modulesSql, new { CourseId = request.CourseId.Value }, cancellationToken: cancellationToken));

        List<ModuleRow> modulesList = modules.ToList();

        if (modulesList.Count == 0)
        {
            return Result.Success<IReadOnlyList<ModuleDetailsDto>>(new List<ModuleDetailsDto>());
        }

        // Get all lessons for all modules in a single query
        List<Guid> moduleIds = modulesList.Select(m => m.Id).ToList();

        const string lessonsSql = @"
            SELECT 
                id,
                module_id AS ModuleId,
                title,
                index,
                duration,
                thumbnail_image_url AS ThumbnailImageUrl,
                access
            FROM lessons
            WHERE module_id = ANY(@ModuleIds)
            ORDER BY module_id, index";

        IEnumerable<Courses.Application.Shared.Dtos.LessonRow> lessons = await connection.QueryAsync<Courses.Application.Shared.Dtos.LessonRow>(
            new CommandDefinition(lessonsSql, new { ModuleIds = moduleIds.ToArray() }, cancellationToken: cancellationToken));

        var lessonsByModuleId = lessons.GroupBy(l => l.ModuleId).ToDictionary(g => g.Key, g => g.ToList());

        List<ModuleDetailsDto> moduleDtos = modulesList.Select(module =>
        {
            List<Courses.Application.Shared.Dtos.LessonRow> moduleLessons = lessonsByModuleId.GetValueOrDefault(module.Id, new List<Courses.Application.Shared.Dtos.LessonRow>());

            List<LessonSummaryDto> lessonDtos = moduleLessons
                .OrderBy(l => l.Index)
                .Select(lesson => new LessonSummaryDto(
                    new CourseId(module.CourseId),
                    new ModuleId(module.Id),
                    new LessonId(lesson.Id),
                    new Title(lesson.Title ?? string.Empty),
                    lesson.Index,
                    lesson.Duration,
                    lesson.ThumbnailImageUrl,
                    Enum.Parse<LessonAccess>(lesson.Access, ignoreCase: true)))
                .ToList();

            TimeSpan totalDuration = TimeSpan.FromTicks(moduleLessons.Sum(l => l.Duration.Ticks));

            return new ModuleDetailsDto(
                new ModuleId(module.Id),
                new Title(module.Title),
                module.Index,
                moduleLessons.Count,
                totalDuration,
                lessonDtos);
        }).ToList();

        return Result.Success<IReadOnlyList<ModuleDetailsDto>>(moduleDtos);
    }

    private sealed record CourseStatusRow(
        string Status,
        int LessonCount
    );
}
