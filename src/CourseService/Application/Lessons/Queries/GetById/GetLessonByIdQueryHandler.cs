using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Courses.Queries.Dtos;
using Domain.Lessons.Errors;
using Kernel;
using Microsoft.EntityFrameworkCore;

namespace Application.Lessons.Queries.GetById;

public class GetLessonByIdQueryHandler : IQueryHandler<GetLessonByIdQuery, LessonReadDto>
{
    private readonly IReadDbContext _dbContext;

    public GetLessonByIdQueryHandler(IReadDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<LessonReadDto>> Handle(
        GetLessonByIdQuery request,
        CancellationToken cancellationToken = default)
    {
        LessonReadDto? result = await _dbContext.Lessons
            .Where(lesson => lesson.Id == request.Id)
            .Select(lesson => new LessonReadDto(
                lesson.Id,
                lesson.Title,
                lesson.Description,
                lesson.VideoUrl,
                lesson.ThumbnailImage,
                lesson.IsPreview,
                lesson.Order,
                lesson.Duration))
            .FirstOrDefaultAsync(cancellationToken);

        if (result is null)
        {
            return Result.Failure<LessonReadDto>(LessonErrors.NotFound);
        }

        return Result.Success(result);
    }
}
