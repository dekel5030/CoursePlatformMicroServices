using System.Data;
using Courses.Application.Abstractions.Data;
using Courses.Application.Abstractions.Storage;
using Courses.Application.Actions;
using Courses.Application.Lessons.Dtos;
using Courses.Application.Lessons.Primitives;
using Courses.Application.Shared.Dtos;
using Courses.Domain.Courses.Primitives;
using Courses.Domain.Lessons.Errors;
using Courses.Domain.Lessons.Primitives;
using Courses.Domain.Shared.Primitives;
using Dapper;
using Kernel;
using Kernel.Messaging.Abstractions;

namespace Courses.Application.Lessons.Queries.GetById;

public class GetLessonByIdQueryHandler : IQueryHandler<GetLessonByIdQuery, LessonDetailsDto>
{
    private readonly ISqlConnectionFactory _connectionFactory;
    private readonly IStorageUrlResolver _urlResolver;

    public GetLessonByIdQueryHandler(
        ISqlConnectionFactory connectionFactory,
        IStorageUrlResolver urlResolver)
    {
        _connectionFactory = connectionFactory;
        _urlResolver = urlResolver;
    }

    public async Task<Result<LessonDetailsDto>> Handle(
        GetLessonByIdQuery request,
        CancellationToken cancellationToken = default)
    {
        using IDbConnection connection = _connectionFactory.CreateConnection();

        // Single query to get lesson with course info via module
        const string sql = @"
            SELECT 
                l.id AS LessonId,
                l.module_id AS ModuleId,
                l.title AS LessonTitle,
                l.description AS LessonDescription,
                l.index AS LessonIndex,
                l.duration AS LessonDuration,
                l.thumbnail_image_url AS ThumbnailImageUrl,
                l.video_url AS VideoUrl,
                l.access AS LessonAccess,
                c.id AS CourseId,
                c.instructor_id AS InstructorId,
                c.status AS CourseStatus,
                c.lesson_count AS CourseLessonCount
            FROM lessons l
            INNER JOIN modules m ON l.module_id = m.id
            INNER JOIN courses c ON m.course_id = c.id
            WHERE l.id = @LessonId";

        LessonDetailRow? row = await connection.QueryFirstOrDefaultAsync<LessonDetailRow>(
            new CommandDefinition(sql, new { LessonId = request.LessonId.Value }, cancellationToken: cancellationToken));

        if (row is null)
        {
            return Result.Failure<LessonDetailsDto>(LessonErrors.NotFound);
        }

        var courseStatus = Enum.Parse<CourseStatus>(row.CourseStatus, ignoreCase: true);

        var courseContext = new CoursePolicyContext(
            new CourseId(row.CourseId),
            new Domain.Shared.Primitives.UserId(row.InstructorId),
            courseStatus,
            row.CourseLessonCount);

        var response = new LessonDetailsDto(
            CourseContext: courseContext,
            CourseId: new CourseId(row.CourseId),
            ModuleId: new Domain.Module.Primitives.ModuleId(row.ModuleId),
            LessonId: new LessonId(row.LessonId),
            Title: new Title(row.LessonTitle),
            Description: new Description(row.LessonDescription ?? string.Empty),
            Index: row.LessonIndex,
            Duration: row.LessonDuration,
            ThumbnailUrl: row.ThumbnailImageUrl != null
                ? _urlResolver.Resolve(StorageCategory.Public, row.ThumbnailImageUrl).Value
                : null,
            Access: Enum.Parse<LessonAccess>(row.LessonAccess, ignoreCase: true),
            Status: courseStatus == CourseStatus.Published
                ? LessonStatus.Published
                : LessonStatus.Draft,
            VideoUrl: row.VideoUrl != null
                ? _urlResolver.Resolve(StorageCategory.Public, row.VideoUrl).Value
                : null
        );

        return Result.Success(response);
    }

    private sealed record LessonDetailRow(
        Guid LessonId,
        Guid ModuleId,
        string LessonTitle,
        string? LessonDescription,
        int LessonIndex,
        TimeSpan LessonDuration,
        string? ThumbnailImageUrl,
        string? VideoUrl,
        string LessonAccess,
        Guid CourseId,
        Guid InstructorId,
        string CourseStatus,
        int CourseLessonCount
    );
}
