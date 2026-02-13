using Courses.Application.Abstractions.Data;
using Courses.Application.Features.LessonPage;
using Courses.Application.Features.Shared;
using Courses.Application.Features.Shared.Mappers;
using Courses.Application.Services.LinkProvider;
using Courses.Domain.Courses;
using Courses.Domain.Courses.Errors;
using Courses.Domain.Courses.Primitives;
using Courses.Domain.Lessons;
using Courses.Domain.Lessons.Errors;
using Courses.Domain.Lessons.Primitives;
using Kernel;
using Kernel.Auth.Abstractions;
using Kernel.Messaging.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Courses.Application.Features.Management.ManagedLessonPage;

internal sealed class ManagedLessonPageQueryHandler
    : IQueryHandler<ManagedLessonPageQuery, LessonPageDto>
{
    private readonly IWriteDbContext _writeDbContext;
    private readonly ILessonPageDtoMapper _lessonPageDtoMapper;
    private readonly IUserContext _userContext;

    public ManagedLessonPageQueryHandler(
        IWriteDbContext writeDbContext,
        ILessonPageDtoMapper lessonPageDtoMapper,
        IUserContext userContext)
    {
        _writeDbContext = writeDbContext;
        _lessonPageDtoMapper = lessonPageDtoMapper;
        _userContext = userContext;
    }

    public async Task<Result<LessonPageDto>> Handle(
        ManagedLessonPageQuery request,
        CancellationToken cancellationToken = default)
    {
        var lessonId = new LessonId(request.LessonId);

        Lesson? lesson = await _writeDbContext.Lessons
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.Id == lessonId, cancellationToken);

        if (lesson == null)
        {
            return Result.Failure<LessonPageDto>(LessonErrors.NotFound);
        }

        Course? course = await _writeDbContext.Courses
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == lesson.CourseId, cancellationToken);

        if (course == null)
        {
            return Result.Failure<LessonPageDto>(LessonErrors.NotFound);
        }

        Result<UserId> authResult = InstructorAuthorization.EnsureInstructorAuthorized(
            _userContext,
            course.InstructorId);

        if (authResult.IsFailure)
        {
            return Result.Failure<LessonPageDto>(CourseErrors.Unauthorized);
        }

        CourseContext courseContext = new(
            course.Id,
            course.InstructorId,
            course.Status,
            IsManagementView: true);

        ModuleContext moduleContext = new(courseContext, lesson.ModuleId);
        LessonContext lessonContext = new(moduleContext, lesson.Id, lesson.Access, HasEnrollment: false);

        LessonPageDto dto = _lessonPageDtoMapper.Map(lesson, course.Title.Value, lessonContext);

        return Result.Success(dto);
    }
}
