using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Enrollments;
using Domain.Enrollments.Errors;
using Domain.Enrollments.Primitives;
using Domain.Users.Primitives;
using Domain.Courses.Primitives;
using Kernel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Enrollments.Commands.CreateEnrollment;

public sealed class CreateEnrollmentCommandHandler
    : ICommandHandler<CreateEnrollmentCommand, EnrollmentId>
{
    private readonly IWriteDbContext _dbContext;
    private readonly ILogger<CreateEnrollmentCommandHandler> _logger;

    public CreateEnrollmentCommandHandler(
        IWriteDbContext dbContext,
        ILogger<CreateEnrollmentCommandHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<Result<EnrollmentId>> Handle(
        CreateEnrollmentCommand command,
        CancellationToken cancellationToken = default)
    {
        var userId = new ExternalUserId(command.UserId);
        var courseId = new ExternalCourseId(command.CourseId);

        // Check if enrollment already exists
        var existingEnrollment = await _dbContext.Enrollments
            .FirstOrDefaultAsync(e => e.UserId == userId && e.CourseId == courseId, cancellationToken);

        if (existingEnrollment is not null)
        {
            _logger.LogWarning("Enrollment already exists for UserId: {UserId}, CourseId: {CourseId}", 
                command.UserId, command.CourseId);
            return Result.Failure<EnrollmentId>(EnrollmentErrors.EnrollmentAlreadyExists);
        }

        // Check if user exists
        var userExists = await _dbContext.KnownUsers
            .AnyAsync(u => u.UserId == userId, cancellationToken);

        if (!userExists)
        {
            _logger.LogWarning("User {UserId} does not exist", command.UserId);
            return Result.Failure<EnrollmentId>(new Error(
                "User.NotFound",
                "The user with the specified identifier was not found",
                ErrorType.NotFound));
        }

        // Check if course exists
        var courseExists = await _dbContext.KnownCourses
            .AnyAsync(c => c.CourseId == courseId, cancellationToken);

        if (!courseExists)
        {
            _logger.LogWarning("Course {CourseId} does not exist", command.CourseId);
            return Result.Failure<EnrollmentId>(new Error(
                "Course.NotFound",
                "The course with the specified identifier was not found",
                ErrorType.NotFound));
        }

        // Create enrollment
        var enrollment = Enrollment.Create(userId, courseId, command.ExpiresAt);

        _dbContext.Enrollments.Add(enrollment);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Enrollment {EnrollmentId} created for UserId: {UserId}, CourseId: {CourseId}",
            enrollment.Id.Value, command.UserId, command.CourseId);

        return Result.Success(enrollment.Id);
    }
}
