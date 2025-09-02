namespace Application.Orders.Commands.SubmitOrder;

public sealed record SubmitOrderDto
{
    public string ExternalUserId { get; set; } = null!;
    public IReadOnlyList<SubmitOrderItemDto> Products { get; set; } = [];
}