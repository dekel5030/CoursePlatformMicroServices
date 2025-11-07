using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Courses.Queries.Dtos;
using Kernel;
using Microsoft.EntityFrameworkCore;

namespace Application.Courses.Queries.GetFeatured;

public class GetFeaturedCoursesQueryHandler : IQueryHandler<GetFeaturedCoursesQuery, IEnumerable<CourseReadDto>>
{
    private readonly IReadDbContext _dbContext;

    public GetFeaturedCoursesQueryHandler(IReadDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<IEnumerable<CourseReadDto>>> Handle(
        GetFeaturedCoursesQuery request, 
        CancellationToken cancellationToken = default)
    {
        List<CourseReadDto> result = await _dbContext.Courses
            .Where(course => course.IsFeatured && course.IsPublished)
            .Select(course => new CourseReadDto(
                course.Id,
                course.Title,
                course.Description,
                course.IsPublished,
                course.IsFeatured,
                course.ImageUrl,
                course.InstructorUserId,
                course.Price,
                course.UpdatedAtUtc))
            .ToListAsync(cancellationToken);

        return Result.Success<IEnumerable<CourseReadDto>>(result);
    }
}
