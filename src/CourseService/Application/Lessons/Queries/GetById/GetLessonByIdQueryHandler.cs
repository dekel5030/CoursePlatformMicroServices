using Courses.Application.Abstractions.Data;
using Courses.Application.Abstractions.Storage;
using Courses.Application.Lessons.Extensions;
using Courses.Application.Lessons.Queries.Dtos;
using Courses.Domain.Lessons;
using Courses.Domain.Lessons.Errors;
using Kernel;
using Kernel.Messaging.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Courses.Application.Lessons.Queries.GetById;

public class GetLessonByIdQueryHandler : IQueryHandler<GetLessonByIdQuery, LessonDetailsDto>
{
    private readonly IReadDbContext _dbContext;
    private readonly IStorageUrlResolver _urlResolver;

    public GetLessonByIdQueryHandler(IReadDbContext dbContext, IStorageUrlResolver urlResolver)
    {
        _dbContext = dbContext;
        _urlResolver = urlResolver;
    }

    public async Task<Result<LessonDetailsDto>> Handle(
        GetLessonByIdQuery request,
        CancellationToken cancellationToken = default)
    {
        Lesson? lesson = await _dbContext.Lessons
            .FirstOrDefaultAsync(l => l.Id == request.Id, cancellationToken);

        if (lesson is null)
        {
            return Result.Failure<LessonDetailsDto>(LessonErrors.NotFound);
        }

        var response = await lesson.ToDetailsDtoAsync(_urlResolver);

        return Result.Success(response);
    }
}