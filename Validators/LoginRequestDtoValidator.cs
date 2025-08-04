using AuthService.Dtos;
using Common.Errors;
using Common.Resources.ValidationMessages;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace AuthService.Validators;

public class LoginRequestDtoValidator : AbstractValidator<LoginRequestDto>
{
    public LoginRequestDtoValidator(IStringLocalizer<ValidationMessages> localizer)
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage(localizer[ValidationMessages.EmailRequired])
            .EmailAddress().WithMessage(localizer[ValidationMessages.EmailInvalidFormat]);

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage(localizer[ValidationMessages.PasswordRequired])
            .MinimumLength(8).WithMessage(localizer[ValidationMessages.PasswordTooShort]);
    }
}