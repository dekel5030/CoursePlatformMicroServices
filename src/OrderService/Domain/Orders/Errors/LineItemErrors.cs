using SharedKernel;

namespace Domain.Orders.Errors;

public static class LineItemErrors
{
    public static readonly Error InvalidQuantity =
        Error.Validation("Orders.LineItem.InvalidQuantity", "Quantity must be bigger than 0");

    public static readonly Error InvalidName =
        Error.Validation("Orders.LineItem.InvalidName", "Name is required");

    public static readonly Error InvalidPrice =
        Error.Validation("Orders.LineItem.InvalidPrice", "Unit price must be bigger or equal than 0");
}