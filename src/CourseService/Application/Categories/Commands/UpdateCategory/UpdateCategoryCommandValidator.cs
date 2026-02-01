using FluentValidation;

namespace Courses.Application.Categories.Commands.UpdateCategory;

public class UpdateCategoryCommandValidator : AbstractValidator<UpdateCategoryCommand>
{
    public UpdateCategoryCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name cannot be empty.")
            .WithErrorCode("Category.NameEmpty")
            .MaximumLength(200)
            .WithMessage("Name is too long.");
    }
}
