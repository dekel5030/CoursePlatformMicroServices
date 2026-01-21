using Courses.Application.Abstractions.Data;
using Courses.Application.Abstractions.Storage;
using Courses.Application.Courses.Dtos;
using Courses.Application.Shared.Extensions;
using Courses.Domain.Courses.Errors;
using Kernel;
using Kernel.Messaging.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Courses.Application.Courses.Queries.GetById;

internal sealed class GetCourseByIdQueryHandler : IQueryHandler<GetCourseByIdQuery, CourseDetailsDto>
{
    private readonly IReadDbContext _dbContext;
    private readonly IStorageUrlResolver _urlResolver;

    public GetCourseByIdQueryHandler(
        IReadDbContext dbContext,
        IStorageUrlResolver urlResolver)
    {
        _dbContext = dbContext;
        _urlResolver = urlResolver;
    }

    public async Task<Result<CourseDetailsDto>> Handle(
        GetCourseByIdQuery request,
        CancellationToken cancellationToken = default)
    {
        CourseDetailsDto? course = await _dbContext.Courses
            .Where(c => c.Id == request.Id)
            .Select(ProjectionMappings.ToCourseDetails)
            .FirstOrDefaultAsync(cancellationToken);

        if (course is null)
        {
            return Result.Failure<CourseDetailsDto>(CourseErrors.NotFound);
        }

        CourseDetailsDto response = course.EnrichWithUrls(_urlResolver);

        return Result.Success(response);
    }
}
