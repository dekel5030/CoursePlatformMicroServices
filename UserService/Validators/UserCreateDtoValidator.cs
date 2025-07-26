using FluentValidation;
using Microsoft.Extensions.Localization;
using UserService.Dtos;
using UserService.Settings;
using UserService.Resources;

namespace UserService.Validators
{
    public class UserCreateDtoValidator : AbstractValidator<UserCreateDto>
    {
        public UserCreateDtoValidator(IStringLocalizer<ValidationMessages> localizer)
        {
            var minPassLen = ValidationSettings.PasswordMinLength;
            var fullNameMinLen = ValidationSettings.FullNameMinLength;
            var fullNameMaxLen = ValidationSettings.FullNameMaxLength;

            RuleFor(user => user.FullName)
                .NotEmpty()
                .WithMessage(localizer["FullNameRequired"])
                .Length(fullNameMinLen, fullNameMaxLen)
                .WithMessage(localizer["FullNameInvalidLength", fullNameMinLen, fullNameMaxLen]);

            RuleFor(user => user.Email)
                .NotEmpty()
                .WithMessage(localizer["EmailRequired"])
                .EmailAddress()
                .WithMessage(localizer["EmailInvalid"]);

            RuleFor(user => user.Password)
                .NotEmpty()
                .WithMessage(localizer["PasswordRequired"])
                .MinimumLength(minPassLen)
                .WithMessage(localizer["PasswordTooShort", minPassLen]);

            RuleFor(user => user.ConfirmPassword)
                .NotEmpty()
                .WithMessage(localizer["ConfirmPasswordRequired"])
                .Equal(user => user.Password)
                .WithMessage(localizer["ConfirmPasswordDoesNotMatch"]);
        }
    }
}
