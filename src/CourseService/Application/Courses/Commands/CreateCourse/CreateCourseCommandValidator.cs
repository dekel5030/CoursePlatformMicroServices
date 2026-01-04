using FluentValidation;

namespace Courses.Application.Courses.Commands.CreateCourse;

public class CreateCourseCommandValidator : AbstractValidator<CreateCourseCommand>
{
    public CreateCourseCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
                .WithMessage("Title is required.")
                .WithErrorCode("course.title.required")
            .MaximumLength(100)
                .WithMessage("Title is too long. Maximum allowed is {MaxLength} characters.")
                .WithErrorCode("course.title.max_length");

        RuleFor(x => x.Description)
            .NotEmpty()
                .WithMessage("Description is required.")
                .WithErrorCode("course.description.required")
            .MaximumLength(2000)
                .WithMessage("Description is too long. Maximum allowed is {MaxLength} characters.")
                .WithErrorCode("course.description.max_length");

        RuleFor(x => x.InstructorId)
            .NotEmpty()
                .WithMessage("A valid Instructor ID must be provided.")
                .WithErrorCode("course.instructor_id.required")
            .NotEqual(Guid.Empty)
                .WithMessage("Instructor ID cannot be empty.")
                .WithErrorCode("course.instructor_id.empty");
    }
}