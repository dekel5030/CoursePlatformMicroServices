using FluentValidation;

namespace Courses.Application.Courses.Commands.CreateCourse;

public class CreateCourseCommandValidator : AbstractValidator<CreateCourseCommand>
{
    public CreateCourseCommandValidator()
    {
        //// Title validation
        //RuleFor(x => x.Title)
        //    .NotNull()
        //    .WithMessage("Title is required.");

        //When(x => x.Title is not null, () =>
        //{
        //    RuleFor(x => x.Title!.Value)
        //        .NotEmpty()
        //            .WithMessage("Title cannot be empty.")
        //            .WithErrorCode("course.title.required")
        //        .MaximumLength(100)
        //            .WithMessage("Title is too long. Maximum allowed is {MaxLength} characters.")
        //            .WithErrorCode("course.title.max_length");
        //});

        //// Description validation
        //RuleFor(x => x.Description)
        //    .NotNull()
        //    .WithMessage("Description is required.");

        //When(x => x.Description is not null, () =>
        //{
        //    RuleFor(x => x.Description!.Value)
        //        .NotEmpty()
        //            .WithMessage("Description is required.")
        //            .WithErrorCode("course.description.required")
        //        .MaximumLength(2000)
        //            .WithMessage("Description is too long. Maximum allowed is {MaxLength} characters.")
        //            .WithErrorCode("course.description.max_length");
        //});

        //// InstructorId
        //RuleFor(x => x.InstructorId)
        //    .NotEmpty()
        //    .NotEqual(Guid.Empty);
    }
}
