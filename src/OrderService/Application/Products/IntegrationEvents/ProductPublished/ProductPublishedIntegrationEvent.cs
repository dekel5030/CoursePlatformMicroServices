using Application.Abstractions.Messaging;

namespace Application.Products.IntegrationEvents.ProductPublished;

public record ProductPublishedIntegrationEvent(
    string ExternalId, 
    string Name, 
    decimal Price, 
    string Currency, 
    int AggregateVersion) : IIntegrationEvent;
