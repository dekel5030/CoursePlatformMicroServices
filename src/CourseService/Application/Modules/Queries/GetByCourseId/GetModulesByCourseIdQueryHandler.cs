using Courses.Application.Abstractions.Data;
using Courses.Application.Lessons.Dtos;
using Courses.Application.Lessons.Primitives;
using Courses.Application.Modules.Dtos;
using Courses.Domain.Courses.Primitives;
using Kernel;
using Kernel.Messaging.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Courses.Application.Modules.Queries.GetByCourseId;

internal sealed class GetModulesByCourseIdQueryHandler : IQueryHandler<GetModulesByCourseIdQuery, IReadOnlyList<ModuleDetailsDto>>
{
    private readonly IReadDbContext _dbContext;

    public GetModulesByCourseIdQueryHandler(IReadDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<IReadOnlyList<ModuleDetailsDto>>> Handle(
        GetModulesByCourseIdQuery request,
        CancellationToken cancellationToken = default)
    {
        var modules = await _dbContext.Modules
            .Where(m => m.CourseId == request.CourseId)
            .Include(m => m.Lessons)
            .Include(m => m.Course)
            .OrderBy(m => m.Index)
            .ToListAsync(cancellationToken);

        var moduleDtos = modules.Select(module =>
        {
            var lessonDtos = module.Lessons
                .OrderBy(l => l.Index)
                .Select(lesson => new LessonSummaryDto(
                    module.CourseId,
                    module.Id,
                    lesson.Id,
                    lesson.Title,
                    lesson.Index,
                    lesson.Duration,
                    lesson.ThumbnailImageUrl != null ? lesson.ThumbnailImageUrl.Path : null,
                    module.Course != null && module.Course.Status == Domain.Courses.Primitives.CourseStatus.Published
                        ? LessonStatus.Published
                        : LessonStatus.Draft,
                    lesson.Access))
                .ToList();

            return new ModuleDetailsDto(
                module.Id,
                module.Title.Value,
                module.Index,
                lessonDtos);
        }).ToList();

        return Result.Success<IReadOnlyList<ModuleDetailsDto>>(moduleDtos);
    }
}
