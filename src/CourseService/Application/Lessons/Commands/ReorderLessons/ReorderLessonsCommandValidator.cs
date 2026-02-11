using FluentValidation;

namespace Courses.Application.Lessons.Commands.ReorderLessons;

internal sealed class ReorderLessonsCommandValidator : AbstractValidator<ReorderLessonsCommand>
{
    public ReorderLessonsCommandValidator()
    {
        RuleFor(x => x.ModuleId)
            .NotNull();

        RuleFor(x => x.LessonIds)
            .NotNull()
            .Must(ids => ids != null && ids.Count == ids.Distinct().Count())
            .WithMessage("Lesson IDs must be unique.");
    }
}
