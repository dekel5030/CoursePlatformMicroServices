using FluentValidation;
using Microsoft.Extensions.Localization;
using UserService.Dtos;
using UserService.Options;
using Common.Resources.ValidationMessages;

namespace UserService.Validators
{
    public class UserCreateDtoValidator : AbstractValidator<UserCreateDto>
    {
        public UserCreateDtoValidator(IStringLocalizer<ValidationMessages> localizer)
        {
            var fullNameMinLen = ValidationOptions.FullNameMinLength;
            var fullNameMaxLen = ValidationOptions.FullNameMaxLength;

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

            RuleFor(user => user.PasswordHash)
                .NotEmpty()
                .WithMessage(localizer["PasswordRequired"]);
        }
    }
}
