using FluentValidation;

namespace Courses.Application.Enrollments.Commands.CreateEnrollment;

public class CreateEnrollmentCommandValidator : AbstractValidator<CreateEnrollmentCommand>
{
    public CreateEnrollmentCommandValidator()
    {
        When(x => x.EnrolledAt.HasValue && x.ExpiresAt.HasValue, () =>
        {
            RuleFor(x => x.ExpiresAt)
                .GreaterThan(x => x.EnrolledAt!.Value)
                .WithMessage("Expiration date must be after enrollment date.")
                .WithErrorCode("Enrollment.InvalidExpirationDate");
        });
    }
}
