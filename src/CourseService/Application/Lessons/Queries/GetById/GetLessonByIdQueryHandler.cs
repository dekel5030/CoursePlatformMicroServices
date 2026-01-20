using Courses.Application.Abstractions.Data;
using Courses.Application.Abstractions.Storage;
using Courses.Application.Actions.Abstract;
using Courses.Application.Lessons.Dtos;
using Courses.Application.Shared.Extensions;
using Courses.Domain.Lessons;
using Courses.Domain.Lessons.Errors;
using Courses.Domain.Lessons.Primitives;
using Kernel;
using Kernel.Messaging.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Courses.Application.Lessons.Queries.GetById;

public class GetLessonByIdQueryHandler : IQueryHandler<GetLessonByIdQuery, LessonDetailsDto>
{
    private readonly IReadDbContext _dbContext;
    private readonly IStorageUrlResolver _urlResolver;

    public GetLessonByIdQueryHandler(
        IReadDbContext dbContext,
        IStorageUrlResolver urlResolver)
    {
        _dbContext = dbContext;
        _urlResolver = urlResolver;
    }

    public async Task<Result<LessonDetailsDto>> Handle(
        GetLessonByIdQuery request,
        CancellationToken cancellationToken = default)
    {
        LessonDetailsDto? lesson = await _dbContext.Lessons
            .Where(lesson => lesson.Id == request.LessonId)
            .Select(ProjectionMappings.ToLessonDetails)
            .FirstOrDefaultAsync(cancellationToken);

        if (lesson is null)
        {
            return Result.Failure<LessonDetailsDto>(LessonErrors.NotFound);
        }


        lesson.EnrichWithUrls(_urlResolver);
        return Result.Success(lesson);
    }
}
