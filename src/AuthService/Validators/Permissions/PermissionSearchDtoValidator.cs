using AuthService.Dtos.Permissions;
using Common.Resources.ValidationMessages;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace AuthService.Validators.Permissions;

public class PermissionSearchDtoValidator : AbstractValidator<PermissionSearchDto>
{
    public PermissionSearchDtoValidator(IStringLocalizer<ValidationMessages> localizer)
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0)
            .WithMessage(localizer[ValidationMessages.PageNumberGreaterThanZero]);

        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .WithMessage(localizer[ValidationMessages.PageSizeGreaterThanZero]);
    }
}
