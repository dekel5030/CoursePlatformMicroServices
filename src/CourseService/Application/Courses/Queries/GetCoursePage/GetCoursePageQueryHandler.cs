using Courses.Application.Abstractions.Data;
using Courses.Application.Abstractions.Storage;
using Courses.Application.Courses.Dtos;
using Courses.Application.Courses.ReadModels;
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
        CoursePage? page = await _readDbContext.CoursePages
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

        if (page is null)
        {
            return Result.Failure<CoursePageDto>(CourseErrors.NotFound);
        }

        CoursePageDto pageDto = page.ToDto();

        pageDto = pageDto with
        {
            ImageUrls = pageDto.ImageUrls
                .Select(url => url is null ?
                    string.Empty
                    : _urlResolver.Resolve(StorageCategory.Public, url).Value).ToList()
        };

        return Result.Success(pageDto);
    }
}
