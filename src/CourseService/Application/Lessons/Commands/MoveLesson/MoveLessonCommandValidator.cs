using FluentValidation;

namespace Courses.Application.Lessons.Commands.MoveLesson;

internal sealed class MoveLessonCommandValidator : AbstractValidator<MoveLessonCommand>
{
    public MoveLessonCommandValidator()
    {
        RuleFor(x => x.LessonId)
            .NotNull();

        RuleFor(x => x.TargetModuleId)
            .NotNull();

        RuleFor(x => x.TargetIndex)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Target index must be non-negative.");
    }
}
