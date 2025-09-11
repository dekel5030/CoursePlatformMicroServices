namespace Orders.Contracts;

public sealed record OrderSubmitted(string OrderId, long EntityVersion, string Status);