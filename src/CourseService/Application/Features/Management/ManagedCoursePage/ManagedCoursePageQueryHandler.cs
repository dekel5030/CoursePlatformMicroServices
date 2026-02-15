using Courses.Application.Abstractions.Data;
using Courses.Application.Features.Shared;
using Courses.Domain.Courses.Errors;
using Courses.Domain.Courses.Primitives;
using Kernel;
using Kernel.Auth.Abstractions;
using Kernel.Messaging.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Courses.Application.Features.Management.ManagedCoursePage;

internal sealed class ManagedCoursePageQueryHandler
    : IQueryHandler<ManagedCoursePageQuery, ManagedCoursePageDto>
{
    private readonly IReadDbContext _readDbContext;
    private readonly IUserContext _userContext;
    private readonly IManagedCoursePageComposer _composer;

    public ManagedCoursePageQueryHandler(
        IReadDbContext readDbContext,
        IUserContext userContext,
        IManagedCoursePageComposer composer)
    {
        _readDbContext = readDbContext;
        _userContext = userContext;
        _composer = composer;
    }

    public async Task<Result<ManagedCoursePageDto>> Handle(
        ManagedCoursePageQuery request,
        CancellationToken cancellationToken = default)
    {
        var courseId = new CourseId(request.Id);

        ManagedCoursePageData? data = await LoadCoursePageDataAsync(courseId, cancellationToken);
        if (data == null)
        {
            return Result.Failure<ManagedCoursePageDto>(CourseErrors.NotFound);
        }

        Result<UserId> authResult = InstructorAuthorization
            .EnsureInstructorAuthorized(_userContext, data.Course.InstructorId);
        if (authResult.IsFailure)
        {
            return Result.Failure<ManagedCoursePageDto>(CourseErrors.Unauthorized);
        }

        ManagedCoursePageDto dto = _composer.Compose(data);
        return Result.Success(dto);
    }

    private async Task<ManagedCoursePageData?> LoadCoursePageDataAsync(
        CourseId courseId,
        CancellationToken cancellationToken)
    {
        var raw = await _readDbContext.Courses
            .AsSplitQuery()
            .Where(c => c.Id == courseId)
            .Select(course => new
            {
                Course = course,
                Modules = _readDbContext.Modules
                    .Where(m => m.CourseId == course.Id)
                    .OrderBy(m => m.Index)
                    .ToList(),
                Lessons = _readDbContext.Lessons
                    .Where(l => l.CourseId == course.Id)
                    .OrderBy(l => l.Index)
                    .ToList(),
                Category = _readDbContext.Categories.FirstOrDefault(cat => cat.Id == course.CategoryId)
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (raw == null)
        {
            return null;
        }

        return new ManagedCoursePageData(raw.Course, raw.Modules, raw.Lessons, raw.Category);
    }
}
