using Courses.Application.Abstractions.Data;
using Courses.Domain.Lessons;
using Courses.Domain.Lessons.Errors;
using Courses.Domain.Lessons.Primitives;
using Kernel;
using Kernel.Messaging.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Courses.Application.Lessons.Commands.DeleteLesson;

public class DeleteLessonCommandHandler : ICommandHandler<DeleteLessonCommand>
{
    private readonly IWriteDbContext _dbContext;

    public DeleteLessonCommandHandler(IWriteDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result> Handle(
        DeleteLessonCommand request, 
        CancellationToken cancellationToken = default)
    {
        LessonId lessonId = new LessonId(request.LessonId);
        Lesson? lesson = await _dbContext.Lessons
            .FirstOrDefaultAsync(lesson => lesson.Id == lessonId, cancellationToken);

        if (lesson is null)
        {
            return Result.Failure(LessonErrors.NotFound);
        }

        _dbContext.Lessons.Remove(lesson);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
