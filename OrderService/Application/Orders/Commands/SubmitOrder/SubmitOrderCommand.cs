using Application.Abstractions.Messaging;
using Domain.Orders;
using SharedKernel;

namespace Application.Orders.Commands.SubmitOrder;

public sealed record SubmitOrderCommand(SubmitOrderDto Dto) : ICommand<Guid>;