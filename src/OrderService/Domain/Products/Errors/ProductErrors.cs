using Kernel;

namespace Domain.Products.Errors;

public class ProductErrors
{
    public static readonly Error NotFound =
        Error.NotFound("Orders.Product.NotFound", "Product not found");
}

