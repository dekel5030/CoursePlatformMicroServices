using Kernel;
using SharedKernel;
using SharedKernel.Products;

namespace Application.Orders.Queries.Dtos;

public record LineItemReadDto(
    string ExternalProductId,
    decimal Quantity,
    string Name, 
    Money UnitPrice, 
    Money LineTotal);