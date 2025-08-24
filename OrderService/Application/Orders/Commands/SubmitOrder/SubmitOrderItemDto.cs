namespace Application.Orders.Commands.SubmitOrder;

public sealed record SubmitOrderItemDto
{
    public Guid Id { get; set; }
    public decimal Quantity { get; set; }
}

