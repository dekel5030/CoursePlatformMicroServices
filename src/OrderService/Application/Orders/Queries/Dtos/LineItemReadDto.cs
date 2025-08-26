using SharedKernel;
using SharedKernel.Products;

namespace Application.Orders.Queries.Dtos;

public record LineItemReadDto(
    Guid ProductId,
    decimal Quantity,
    string Name, 
    Money UnitPrice, 
    Money LineTotal);