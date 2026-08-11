namespace SharedContracts;

// Integration Event representing an updated or published price
public record ProductPriceChangedIntegrationEvent(Guid ProductId, string SKU, decimal NewPrice, DateTime PublishedAt);

// Integration Event representing a finalized client checkout order
public record OrderPlacedIntegrationEvent(Guid OrderId, Guid ProductId, int Quantity, decimal TotalPrice, DateTime PlacedAt);
