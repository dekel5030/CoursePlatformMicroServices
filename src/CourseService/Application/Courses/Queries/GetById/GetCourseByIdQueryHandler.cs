using Courses.Application.Abstractions.Data;
using Courses.Application.Abstractions.Storage;
using Courses.Application.Courses.Extensions;
using Courses.Application.Courses.Queries.Dtos;
using Courses.Domain.Courses;
using Courses.Domain.Courses.Errors;
using Courses.Domain.Courses.Primitives;
using Kernel;
using Kernel.Messaging.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Courses.Application.Courses.Queries.GetById;

internal class GetCourseByIdQueryHandler : IQueryHandler<GetCourseByIdQuery, CourseDetailsDto>
{
    private readonly IReadDbContext _dbContext;
    private readonly IStorageUrlResolver _urlResolver;

    public GetCourseByIdQueryHandler(IReadDbContext dbContext, IStorageUrlResolver urlResolver)
    {
        _dbContext = dbContext;
        _urlResolver = urlResolver;
    }

    public async Task<Result<CourseDetailsDto>> Handle(
        GetCourseByIdQuery request,
        CancellationToken cancellationToken = default)
    {
        Course? course = await _dbContext.Courses
            .Include(c => c.Lessons)
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

        if (course is null)
        {
            return Result.Failure<CourseDetailsDto>(CourseErrors.NotFound);
        }

        CourseDetailsDto response = await course.ToDetailsDtoAsync(_urlResolver, cancellationToken);

        return Result.Success(response);
    }
}