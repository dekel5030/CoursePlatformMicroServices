namespace Application.Orders.Commands.SubmitOrder;

public sealed record SubmitOrderDto(string ExternalUserId, IReadOnlyList<SubmitOrderItemDto> Products);