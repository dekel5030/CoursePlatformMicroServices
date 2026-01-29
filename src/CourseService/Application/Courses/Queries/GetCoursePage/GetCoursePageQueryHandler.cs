using Courses.Application.Abstractions.Data;
using Courses.Application.Abstractions.Data.ReadModels;
using Courses.Application.Abstractions.Storage;
using Courses.Application.Courses.Dtos;
using Courses.Domain.Courses.Errors;
using Kernel;
using Kernel.Messaging.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Courses.Application.Courses.Queries.GetCoursePage;

internal sealed class GetCoursePageQueryHandler
    : IQueryHandler<GetCoursePageQuery, CoursePageDto>
{
    private readonly IReadDbContext _readDbContext;
    private readonly IStorageUrlResolver _urlResolver;

    public GetCoursePageQueryHandler(
        IReadDbContext readDbContext,
        IStorageUrlResolver urlResolver)
    {
        _readDbContext = readDbContext;
        _urlResolver = urlResolver;
    }

    public async Task<Result<CoursePageDto>> Handle(
        GetCoursePageQuery request,
        CancellationToken cancellationToken = default)
    {
        CourseHeaderReadModel? header = await _readDbContext.CourseHeaders
            .FirstOrDefaultAsync(h => h.Id == request.Id, cancellationToken);

        if (header is null)
        {
            return Result.Failure<CoursePageDto>(CourseErrors.NotFound);
        }

        CourseStructureReadModel? structure = await _readDbContext.CourseStructures
            .FirstOrDefaultAsync(s => s.CourseId == request.Id, cancellationToken);

        CourseStatsReadModel? stats = await _readDbContext.CourseStats
            .FirstOrDefaultAsync(s => s.CourseId == request.Id, cancellationToken);

        CoursePageDto pageDto = ComposeCoursePageDto(header, structure, stats);

        pageDto = pageDto with
        {
            ImageUrls = pageDto.ImageUrls
                .Select(url => url is null ?
                    string.Empty
                    : _urlResolver.Resolve(StorageCategory.Public, url).Value).ToList()
        };

        return Result.Success(pageDto);
    }

    private static CoursePageDto ComposeCoursePageDto(
        CourseHeaderReadModel header,
        CourseStructureReadModel? structure,
        CourseStatsReadModel? stats)
    {
        List<ModuleDto> modules = structure?.Modules.Select(m => new ModuleDto
        {
            Id = m.Id,
            Title = m.Title,
            Index = m.Index,
            Duration = m.Lessons.Sum(l => l.Duration.Ticks) > 0
                ? TimeSpan.FromTicks(m.Lessons.Sum(l => l.Duration.Ticks))
                : TimeSpan.Zero,
            LessonCount = m.Lessons.Count,
            Lessons = m.Lessons.Select(l => new LessonDto
            {
                Id = l.Id,
                Title = l.Title,
                Index = l.Index,
                Duration = l.Duration,
                ThumbnailUrl = l.ThumbnailUrl,
                Access = l.Access,
                Links = []
            }).ToList(),
            Links = []
        }).ToList() ?? [];

        return new CoursePageDto
        {
            Id = header.Id,
            Title = header.Title,
            Description = header.Description,
            InstructorId = header.InstructorId,
            InstructorName = header.InstructorName,
            InstructorAvatarUrl = header.InstructorAvatarUrl,
            Status = header.Status,
            Price = new Money(header.PriceAmount, header.PriceCurrency),
            EnrollmentCount = stats?.EnrollmentCount ?? 0,
            LessonsCount = stats?.LessonsCount ?? 0,
            TotalDuration = stats?.TotalDuration ?? TimeSpan.Zero,
            UpdatedAtUtc = header.UpdatedAtUtc,
            ImageUrls = header.ImageUrls.AsReadOnly(),
            Tags = header.Tags.AsReadOnly(),
            CategoryId = header.CategoryId,
            CategoryName = header.CategoryName,
            CategorySlug = header.CategorySlug,
            Modules = modules,
            Links = []
        };
    }
}
