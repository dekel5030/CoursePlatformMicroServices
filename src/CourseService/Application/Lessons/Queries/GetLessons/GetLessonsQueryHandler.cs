using Courses.Application.Abstractions.Data;
using Courses.Application.Abstractions.Storage;
using Courses.Application.Lessons.Dtos;
using Courses.Domain.Lessons;
using Courses.Domain.Lessons.Primitives;
using Kernel;
using Kernel.Messaging.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Courses.Application.Lessons.Queries.GetLessons;

internal sealed class GetLessonsQueryHandler : IQueryHandler<GetLessonsQuery, IReadOnlyList<LessonDto>>
{
    private readonly IReadDbContext _readDbContext;
    private readonly IStorageUrlResolver _urlResolver;

    public GetLessonsQueryHandler(
        IReadDbContext readDbContext,
        IStorageUrlResolver urlResolver)
    {
        _readDbContext = readDbContext;
        _urlResolver = urlResolver;
    }

    public async Task<Result<IReadOnlyList<LessonDto>>> Handle(
    GetLessonsQuery request,
    CancellationToken cancellationToken = default)
    {
        IQueryable<Lesson> query = _readDbContext.Lessons;
        query = ApplyFilters(query, request.Filter);

        List<Lesson> lessons = await query
            .OrderBy(lesson => lesson.ModuleId)
            .ThenBy(lesson => lesson.Index)
            .ToListAsync(cancellationToken);

        if (lessons.Count == 0)
        {
            return Result.Success<IReadOnlyList<LessonDto>>([]);
        }

        var response = lessons.Select(lesson => LessonDtoMapper.Map(lesson, ResolveUrl)).ToList();

        return Result.Success<IReadOnlyList<LessonDto>>(response);
    }

    private static IQueryable<Lesson> ApplyFilters(IQueryable<Lesson> query, LessonFilter filter)
    {
        if (filter.CourseId is { } courseId)
        {
            query = query.Where(l => l.CourseId == courseId);
        }

        if (filter.Id is { } id)
        {
            query = query.Where(l => l.Id == id);
        }

        if (filter.Ids is { } ids)
        {
            query = query.Where(l => ids.Select(i => new LessonId(i)).Contains(l.Id));
        }

        return query;
    }

    private string ResolveUrl(string? path)
    {
        return path is not null ? _urlResolver.Resolve(StorageCategory.Public, path).Value : "";
    }
}
