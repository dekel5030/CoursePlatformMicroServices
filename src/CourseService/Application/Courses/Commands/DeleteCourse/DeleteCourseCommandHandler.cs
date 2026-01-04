using Courses.Application.Abstractions.Data;
using Courses.Domain.Courses;
using Courses.Domain.Courses.Errors;
using Courses.Domain.Courses.Primitives;
using Kernel;
using Kernel.Messaging.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Courses.Application.Courses.Commands.DeleteCourse;

public class DeleteCourseCommandHandler : ICommandHandler<DeleteCourseCommand>
{
    private readonly IWriteDbContext _dbContext;

    public DeleteCourseCommandHandler(IWriteDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result> Handle(
        DeleteCourseCommand request, 
        CancellationToken cancellationToken = default)
    {
        CourseId courseId = new CourseId(request.CourseId);

        Course? course = await _dbContext.Courses
            .FirstOrDefaultAsync(course => course.Id == courseId, cancellationToken);
        
        if (course is null)
        {
            return Result.Failure(CourseErrors.NotFound);
        }

        _dbContext.Courses.Remove(course);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
