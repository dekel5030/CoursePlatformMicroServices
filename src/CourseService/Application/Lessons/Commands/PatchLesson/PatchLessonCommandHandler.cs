using Courses.Application.Abstractions.Data;
using Courses.Domain.Lessons;
using Courses.Domain.Lessons.Errors;
using Courses.Domain.Lessons.Primitives;
using Courses.Domain.Shared.Primitives;
using Kernel;
using Kernel.Messaging.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Courses.Application.Lessons.Commands.PatchLesson;

internal class PatchLessonCommandHandler : ICommandHandler<PatchLessonCommand>
{
    private readonly IWriteDbContext _dbContext;

    public PatchLessonCommandHandler(IWriteDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result> Handle(
        PatchLessonCommand request,
        CancellationToken cancellationToken = default)
    {
        var lessonId = new LessonId(request.LessonId);

        var lesson = await _dbContext.Lessons
            .FirstOrDefaultAsync(l => l.Id == lessonId, cancellationToken);

        if (lesson is null)
        {
            return Result.Failure(LessonErrors.NotFound);
        }

        if (request.Title is not null)
        {
            var titleResult = lesson.SetTitle(new Title(request.Title));
            if (titleResult.IsFailure)
            {
                return titleResult;
            }
        }

        if (request.Description is not null)
        {
            var descriptionResult = lesson.SetDescription(new Description(request.Description));
            if (descriptionResult.IsFailure)
            {
                return descriptionResult;
            }
        }

        if (request.Access is not null)
        {
            if (Enum.TryParse<LessonAccess>(request.Access, ignoreCase: true, out var lessonAccess))
            {
                var accessResult = lesson.SetAccess(lessonAccess);
                if (accessResult.IsFailure)
                {
                    return accessResult;
                }
            }
            else
            {
                return Result.Failure(Error.Validation("Lesson.InvalidAccess", "Invalid lesson access value."));
            }
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
