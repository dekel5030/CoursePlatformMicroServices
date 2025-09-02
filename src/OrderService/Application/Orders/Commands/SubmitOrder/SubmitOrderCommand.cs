using Application.Abstractions.Messaging;
using Domain.Orders.Primitives;

namespace Application.Orders.Commands.SubmitOrder;

public sealed record SubmitOrderCommand(SubmitOrderDto Dto) : ICommand<OrderId>;