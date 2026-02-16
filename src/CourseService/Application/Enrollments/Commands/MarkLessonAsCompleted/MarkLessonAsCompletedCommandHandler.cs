using Courses.Application.Abstractions.Data;
using Courses.Domain.Abstractions.Repositories;
using Courses.Domain.Enrollments;
using Courses.Domain.Enrollments.Errors;
using Kernel;
using Kernel.Messaging.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Courses.Application.Enrollments.Commands.MarkLessonAsCompleted;

internal sealed class MarkLessonAsCompletedCommandHandler : ICommandHandler<MarkLessonAsCompletedCommand>
{
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly IWriteDbContext _dbContext;
    private readonly IUnitOfWork _unitOfWork;

    public MarkLessonAsCompletedCommandHandler(
        IEnrollmentRepository enrollmentRepository,
        IWriteDbContext dbContext,
        IUnitOfWork unitOfWork)
    {
        _enrollmentRepository = enrollmentRepository;
        _dbContext = dbContext;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        MarkLessonAsCompletedCommand request,
        CancellationToken cancellationToken = default)
    {
        Enrollment? enrollment = await _enrollmentRepository
            .GetByIdAsync(request.EnrollmentId, cancellationToken);

        if (enrollment is null)
        {
            return Result.Failure(EnrollmentErrors.NotFound);
        }

        bool lessonInCourse = await _dbContext.Lessons
            .AsNoTracking()
            .Where(l => l.Id == request.LessonId && l.CourseId == enrollment.CourseId)
            .AnyAsync(cancellationToken);

        if (!lessonInCourse)
        {
            return Result.Failure(EnrollmentErrors.LessonNotInCourse);
        }

        int totalLessonsInCourse = await _dbContext.Lessons
            .CountAsync(lesson => lesson.CourseId == enrollment.CourseId, cancellationToken);

        enrollment.MarkLessonAsCompleted(request.LessonId, totalLessonsInCourse);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
