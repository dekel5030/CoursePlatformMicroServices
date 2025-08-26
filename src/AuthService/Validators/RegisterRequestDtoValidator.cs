using AuthService.Dtos;
using Common.Resources.ValidationMessages;
using FluentValidation;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace AuthService.Validators;

public class RegisterRequestDtoValidator : AbstractValidator<RegisterRequestDto>
{
    public RegisterRequestDtoValidator(IStringLocalizer<ValidationMessages> localizer,
                                       IOptions<ValidationSettings> settings)
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage(localizer[ValidationMessages.EmailRequired])
            .MaximumLength(settings.Value.EmailMaxLength)
                .WithMessage(localizer[ValidationMessages.EmailTooLong, settings.Value.EmailMaxLength])
            .EmailAddress().WithMessage(localizer[ValidationMessages.EmailInvalidFormat]);

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage(localizer[ValidationMessages.PasswordRequired])
            .MinimumLength(settings.Value.PasswordMinLength)
                .WithMessage(localizer[ValidationMessages.PasswordTooShort, settings.Value.PasswordMinLength])
            .MaximumLength(settings.Value.PasswordMaxLength)
                .WithMessage(localizer[ValidationMessages.PasswordTooLong, settings.Value.PasswordMaxLength])
            .DependentRules(() =>
            {
                if (settings.Value.PasswordRequireUppercase)
                {
                    RuleFor(x => x.Password)
                        .Matches(@"[A-Z]")
                        .WithMessage(localizer[ValidationMessages.PasswordMustContainUppercase])
                        .When(x => !string.IsNullOrEmpty(x.Password));
                }

                if (settings.Value.PasswordRequireLowercase)
                {
                    RuleFor(x => x.Password)
                        .Matches(@"[a-z]")
                        .WithMessage(localizer[ValidationMessages.PasswordMustContainLowercase])
                        .When(x => !string.IsNullOrEmpty(x.Password));
                }

                if (settings.Value.PasswordRequireDigit)
                {
                    RuleFor(x => x.Password)
                        .Matches(@"\d")
                        .WithMessage(localizer[ValidationMessages.PasswordMustContainDigit])
                        .When(x => !string.IsNullOrEmpty(x.Password));
                }

                if (settings.Value.PasswordRequireSpecial)
                {
                    RuleFor(x => x.Password)
                        .Matches(@"[!@#$%^&*(),.?""{}|<>_\[\]\\/\-+=;:]")
                        .WithMessage(localizer[ValidationMessages.PasswordMustContainSpecial])
                        .When(x => !string.IsNullOrEmpty(x.Password));
                }
            });

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty().WithMessage(localizer[ValidationMessages.ConfirmPasswordRequired])
            .MaximumLength(settings.Value.PasswordMaxLength)
                .WithMessage(localizer[ValidationMessages.ConfirmPasswordTooLong, settings.Value.PasswordMaxLength])
            .Equal(x => x.Password).WithMessage(localizer[ValidationMessages.PasswordMismatch]);

        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage(localizer[ValidationMessages.FullNameRequired])
            .MaximumLength(settings.Value.FullNameMaxLength)
                .WithMessage(localizer[ValidationMessages.FullNameTooLong, settings.Value.FullNameMaxLength]);

    }
}
