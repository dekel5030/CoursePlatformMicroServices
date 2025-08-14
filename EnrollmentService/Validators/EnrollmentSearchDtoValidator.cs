using Common.Resources.ValidationMessages;
using EnrollmentService.Dtos;
using EnrollmentService.Options;
using FluentValidation;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace EnrollmentService.Validators;

public class EnrollmentSearchDtoValidator : AbstractValidator<EnrollmentSearchDto>
{
    public EnrollmentSearchDtoValidator(
        IStringLocalizer<ValidationMessages> localizer, IOptions<PaginationOptions> paginationOptions)
    {
        RuleFor(x => x.Status)
            .IsInEnum().When(x => x.Status.HasValue)
            .WithMessage(localizer[ValidationMessages.InvalidEnrollmentStatus]);

        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1)
            .WithMessage(localizer[ValidationMessages.PageNumberGreaterThanZero]);

        RuleFor(x => x.PageSize)
            .GreaterThanOrEqualTo(1)
            .WithMessage(localizer[ValidationMessages.PageSizeGreaterThanZero])
            .LessThanOrEqualTo(paginationOptions.Value.MaxPageSize)
            .WithMessage(localizer[ValidationMessages.PageSizeLessThanOrEqualTo, paginationOptions.Value.MaxPageSize]);

        RuleFor(x => x.Id).GreaterThan(0).When(x => x.Id.HasValue)
            .WithMessage(localizer[ValidationMessages.EnrollmentIdGreaterThanZero]);

        RuleFor(x => x.CourseId).GreaterThan(0).When(x => x.CourseId.HasValue)
            .WithMessage(localizer[ValidationMessages.CourseIdGreaterThanZero]);

        RuleFor(x => x.UserId).GreaterThan(0).When(x => x.UserId.HasValue)
            .WithMessage(localizer[ValidationMessages.UserIdGreaterThanZero]);
    }
}