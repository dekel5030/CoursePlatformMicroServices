using SharedKernel;
using SharedKernel.Products;

namespace Application.Orders.Queries.Dtos;

public record LineItemReadDto(
    Guid ProductId,
    decimal Quantity,
    Sku Sku,
    string Name, 
    Money UnitPrice, 
    Money LineTotal);