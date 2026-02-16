using FluentValidation;

namespace Courses.Application.Enrollments.Commands.UpdateLessonProgress;

public sealed class UpdateLessonProgressCommandValidator : AbstractValidator<UpdateLessonProgressCommand>
{
    public UpdateLessonProgressCommandValidator()
    {
        RuleFor(x => x.Seconds)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Seconds must be non-negative.")
            .WithErrorCode("Enrollment.InvalidProgressSeconds");
    }
}
