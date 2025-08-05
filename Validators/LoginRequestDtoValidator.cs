using AuthService.Dtos;
using Common.Resources.ValidationMessages;
using FluentValidation;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace AuthService.Validators;

public class LoginRequestDtoValidator : AbstractValidator<LoginRequestDto>
{
    public LoginRequestDtoValidator(IStringLocalizer<ValidationMessages> localizer,
                                    IOptions<ValidationSettings> settings)
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage(localizer[ValidationMessages.EmailRequired])
            .MaximumLength(settings.Value.EmailMaxLength).WithMessage(localizer[ValidationMessages.EmailTooLong])
            .EmailAddress().WithMessage(localizer[ValidationMessages.EmailInvalidFormat]);

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage(localizer[ValidationMessages.PasswordRequired])
            .MinimumLength(settings.Value.PasswordMinLength).WithMessage(localizer[ValidationMessages.PasswordTooShort])
            .MaximumLength(settings.Value.PasswordMaxLength).WithMessage(localizer[ValidationMessages.PasswordTooLong]);
    }
}