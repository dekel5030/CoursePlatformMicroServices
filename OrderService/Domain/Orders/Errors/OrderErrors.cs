using SharedKernel;

namespace Domain.Orders.Errors;

public static class OrderErrors
{
    public static readonly Error NotFound =
        Error.NotFound("Orders.Order.NotFound", "Order not found");
}