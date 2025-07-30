using FluentValidation;
using Microsoft.Extensions.Localization;
using Common.Errors;
using Common.Resources.ValidationMessages;

namespace UserService.Dtos
{
    public class UserSearchDtoValidator : AbstractValidator<UserSearchDto>
    {
        public UserSearchDtoValidator(IStringLocalizer<ValidationMessages> localizer)
        {
            var invalidPageNumMsg = localizer[ErrorCode.InvalidPageNumber.ToString()];
            var invalidPageSizeMsg = localizer[ErrorCode.InvalidPageSize.ToString()];

            RuleFor(x => x.PageNumber).GreaterThan(0).WithMessage(invalidPageNumMsg);
            RuleFor(x => x.PageSize).GreaterThan(0).WithMessage(invalidPageSizeMsg);
        }
    }
}