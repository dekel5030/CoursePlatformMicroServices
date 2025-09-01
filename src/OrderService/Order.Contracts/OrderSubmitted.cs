namespace Orders.Contracts;

public sealed record OrderSubmitted
{
    public string OrderId { get; private set; } = null!;
    public string UserId { get; private set; } = null!;
    public string Status { get; private set; } = null!;

    private OrderSubmitted() { }

    public static OrderSubmitted Create(string orderId, string userId, string status) =>
        new OrderSubmitted
        {
            OrderId = orderId,
            UserId = userId,
            Status = status
        };

    public OrderStatus GetStatus() => Enum.TryParse<OrderStatus>(Status, true, out var parsed)
            ? parsed
            : throw new ArgumentException($"Invalid OrderStatus: {Status}");
}
