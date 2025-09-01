namespace Application.Orders.Commands.SubmitOrder;

public sealed record SubmitOrderDto
{
    public Guid UserId { get; set; }
    public Guid OrderId { get; set; }
    public IReadOnlyList<SubmitOrderItemDto> Products { get; set; } = [];
}

