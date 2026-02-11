using FluentValidation;

namespace Courses.Application.Modules.Commands.ReorderModules;

internal sealed class ReorderModulesCommandValidator : AbstractValidator<ReorderModulesCommand>
{
    public ReorderModulesCommandValidator()
    {
        RuleFor(x => x.CourseId)
            .NotNull();

        RuleFor(x => x.ModuleIds)
            .NotNull()
            .Must(ids => ids != null && ids.Count == ids.Distinct().Count())
            .WithMessage("Module IDs must be unique.");
    }
}
