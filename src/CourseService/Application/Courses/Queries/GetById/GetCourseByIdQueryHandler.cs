using Courses.Application.Abstractions.Data;
using Courses.Application.Courses.Queries.Dtos;
using Courses.Domain.Courses.Errors;
using Kernel;
using Kernel.Messaging.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Courses.Application.Courses.Queries.GetById;

public class GetCourseByIdQueryHandler : IQueryHandler<GetCourseByIdQuery, CourseReadDto>
{
    private readonly IReadDbContext _dbContext;

    public GetCourseByIdQueryHandler(IReadDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<CourseReadDto>> Handle(
        GetCourseByIdQuery request,
        CancellationToken cancellationToken = default)
    {
        CourseReadDto? result = await _dbContext.Courses
            .Where(course => course.Id == request.Id)
            .Select(course => new CourseReadDto(
                course.Id,
                course.Title,
                course.Description,
                course.IsPublished,
                course.ImageUrl,
                course.InstructorUserId,
                course.Price,
                course.UpdatedAtUtc,
                course.Lessons.OrderBy(l => l.Order)
                .Select(lesson => new LessonReadDto(
                    lesson.Id,
                    lesson.Title,
                    lesson.Description,
                    lesson.VideoUrl,
                    lesson.ThumbnailImage,
                    lesson.IsPreview,
                    lesson.Order,
                    lesson.Duration))
                .ToList()
                ))
            .FirstOrDefaultAsync(cancellationToken);

        if (result is null)
        {
            return Result.Failure<CourseReadDto>(CourseErrors.NotFound);
        }

        return Result.Success(result);
    }
}
