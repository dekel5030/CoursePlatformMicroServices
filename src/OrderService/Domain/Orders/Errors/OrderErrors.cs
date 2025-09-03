using Kernel;

namespace Domain.Orders.Errors;

public static class OrderErrors
{
    public static readonly Error NotFound =
        Error.NotFound("Orders.Order.NotFound", "Order not found");

    public static readonly Error OrderIsEmpty =
        Error.Validation("Orders.Order.Empty", "Order cannot be submitted because it is empty");

    public static readonly Error AlreadySubmitted =
        Error.Conflict("Orders.Order.AlreadySubmitted", "Order has already been submitted");
}