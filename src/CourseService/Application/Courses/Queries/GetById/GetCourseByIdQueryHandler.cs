using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Courses.Queries.Dtos;
using Domain.Courses.Errors;
using Kernel;
using Microsoft.EntityFrameworkCore;

namespace Application.Courses.Queries.GetById;

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
                course.UpdatedAtUtc))
            .FirstOrDefaultAsync(cancellationToken);

        if (result is null)
        {
            return Result.Failure<CourseReadDto>(CourseErrors.NotFound);
        }

        return Result.Success(result);
    }
}
