namespace Products.Contracts;

public sealed record ProductPublished(
    string Id,
    string Name,
    decimal Price,
    string Currency
);