using Application.Abstractions.Messaging;

namespace Application.Orders.Commands.SubmitOrder;

public sealed record SubmitOrderCommand(SubmitOrderDto Dto) : ICommand<Guid>;