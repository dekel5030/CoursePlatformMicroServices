using SharedKernel;
using SharedKernel.Orders;
using SharedKernel.Products;

namespace Application.Orders.Queries.Dtos;

public record LineItemReadDto(
    LineItemId Id,
    ProductId ProductId,
    decimal Quantity,
    Sku Sku,
    string Name, 
    Money UnitPrice, 
    Money TotalPrice);