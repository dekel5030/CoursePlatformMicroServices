using FluentValidation;

namespace Application.Orders.Commands.SubmitOrder;

public sealed class SubmitOrderDtoValidator : AbstractValidator<SubmitOrderDto>
{
    public SubmitOrderDtoValidator()
    {

    }
}

