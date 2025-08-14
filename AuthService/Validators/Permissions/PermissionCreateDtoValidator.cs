using AuthService.Dtos.Permissions;
using Common.Resources.ValidationMessages;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace AuthService.Validators.Permissions;

public class PermissionCreateDtoValidator : AbstractValidator<PermissionCreateDto>
{
    public PermissionCreateDtoValidator(IStringLocalizer<ValidationMessages> localizer)
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage(localizer[ValidationMessages.PermissionNameRequired]);
    }
}
