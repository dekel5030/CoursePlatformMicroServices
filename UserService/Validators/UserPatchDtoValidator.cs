using Common.Resources.ValidationMessages;
using FluentValidation;
using Microsoft.Extensions.Localization;
using UserService.Dtos;
using UserService.Settings;

namespace UserService.Validators
{
    public class UserPatchDtoValidator : AbstractValidator<UserPatchDto>
    {
        public UserPatchDtoValidator(IStringLocalizer<ValidationMessages> localizer)
        {
            var fullNameMinLen = ValidationSettings.FullNameMinLength;
            var fullNameMaxLen = ValidationSettings.FullNameMaxLength;
            
            RuleFor(user => user.FullName)
                .NotEmpty()
                .WithMessage(localizer["FullNameRequired"])
                .Length(fullNameMinLen, fullNameMaxLen)
                .WithMessage(localizer["FullNameInvalidLength", fullNameMinLen, fullNameMaxLen]);

            RuleFor(user => user.PhoneNumber)
                .Matches(@"^(\+972|972|05)\d{8}$")
                .WithMessage(localizer["PhoneNumberInvalid"]);

            RuleFor(user => user.DateOfBirth)
                .LessThan(DateTime.Now)
                .WithMessage(localizer["DateOfBirthMustBeInPast"]);

            RuleFor(user => user.Bio)
                .MaximumLength(500)
                .WithMessage(localizer["BioMaxLength"]);
        }
    }
}