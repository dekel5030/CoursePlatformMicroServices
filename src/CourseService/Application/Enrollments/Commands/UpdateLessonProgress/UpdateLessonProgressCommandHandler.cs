using Courses.Application.Abstractions.Data;
using Courses.Domain.Abstractions.Repositories;
using Courses.Domain.Enrollments;
using Courses.Domain.Enrollments.Errors;
using Kernel;
using Kernel.Messaging.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Courses.Application.Enrollments.Commands.UpdateLessonProgress;

internal sealed class UpdateLessonProgressCommandHandler : ICommandHandler<UpdateLessonProgressCommand>
{
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly IWriteDbContext _dbContext;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateLessonProgressCommandHandler(
        IEnrollmentRepository enrollmentRepository,
        IWriteDbContext dbContext,
        IUnitOfWork unitOfWork)
    {
        _enrollmentRepository = enrollmentRepository;
        _dbContext = dbContext;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        UpdateLessonProgressCommand request,
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
            .Where(lesson => lesson.Id == request.LessonId && lesson.CourseId == enrollment.CourseId)
            .AnyAsync(cancellationToken);

        if (!lessonInCourse)
        {
            return Result.Failure(EnrollmentErrors.LessonNotInCourse);
        }

        enrollment.TrackProgress(request.LessonId, request.Seconds);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
