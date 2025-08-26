using Common.Resources.ValidationMessages;
using EnrollmentService.Dtos;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace EnrollmentService.Validators;

public class EnrollmentCreateDtoValidator : AbstractValidator<EnrollmentCreateDto>
{
    public EnrollmentCreateDtoValidator(IStringLocalizer<ValidationMessages> localizer)
    {
        RuleFor(e => e.CourseId)
            .NotEmpty()
            .WithMessage(localizer[ValidationMessages.CourseIdRequired]);

        RuleFor(e => e.UserId)
            .NotEmpty()
            .WithMessage(localizer[ValidationMessages.UserIdRequired]);

        RuleFor(e => e.Status)
            .IsInEnum()
            .WithMessage(localizer[ValidationMessages.InvalidEnrollmentStatus]);

        RuleFor(e => e.ExpiresAt)
            .Must(date => date == null || date > DateTime.UtcNow)
            .WithMessage(localizer[ValidationMessages.ExpirationMustBeInFuture]);
    }
}