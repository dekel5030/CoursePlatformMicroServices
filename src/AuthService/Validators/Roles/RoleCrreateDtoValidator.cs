using AuthService.Dtos.Roles;
using Common.Resources.ValidationMessages;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace AuthService.Validators.Roles;

public class RoleCreateDtoValidator : AbstractValidator<RoleCreateDto>
{
    public RoleCreateDtoValidator(IStringLocalizer<ValidationMessages> localizer)
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage(localizer[ValidationMessages.RoleNameRequired]);
    }
}