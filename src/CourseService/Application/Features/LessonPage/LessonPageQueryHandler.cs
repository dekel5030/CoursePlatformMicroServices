using Courses.Application.Abstractions.Data;
using Courses.Application.Features.Shared.Mappers;
using Courses.Application.Services.LinkProvider;
using Courses.Domain.Courses;
using Courses.Domain.Lessons;
using Courses.Domain.Lessons.Errors;
using Courses.Domain.Lessons.Primitives;
using Kernel;
using Kernel.Messaging.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Courses.Application.Features.LessonPage;

internal sealed class LessonPageQueryHandler : IQueryHandler<LessonPageQuery, LessonPageDto>
{
    private readonly IReadDbContext _readDbContext;
    private readonly ILessonPageDtoMapper _lessonPageDtoMapper;

    public LessonPageQueryHandler(
        IReadDbContext readDbContext,
        ILessonPageDtoMapper lessonPageDtoMapper)
    {
        _readDbContext = readDbContext;
        _lessonPageDtoMapper = lessonPageDtoMapper;
    }

    public async Task<Result<LessonPageDto>> Handle(
        LessonPageQuery request,
        CancellationToken cancellationToken = default)
    {
        var lessonId = new LessonId(request.LessonId);

        Lesson? lesson = await _readDbContext.Lessons
            .FirstOrDefaultAsync(l => l.Id == lessonId, cancellationToken);

        if (lesson == null)
        {
            return Result.Failure<LessonPageDto>(LessonErrors.NotFound);
        }

        Course? course = await _readDbContext.Courses
            .FirstOrDefaultAsync(course => course.Id == lesson.CourseId, cancellationToken);

        if (course == null)
        {
            return Result.Failure<LessonPageDto>(LessonErrors.NotFound);
        }

        CourseContext courseContext = new(
            course.Id,
            course.InstructorId,
            course.Status,
            IsManagementView: false);

        ModuleContext moduleContext = new(courseContext, lesson.ModuleId);
        LessonContext lessonContext = new(moduleContext, lesson.Id, lesson.Access, HasEnrollment: false);

        LessonPageDto dto = _lessonPageDtoMapper.Map(lesson, course.Title.Value, lessonContext);

        return Result.Success(dto);
    }
}
