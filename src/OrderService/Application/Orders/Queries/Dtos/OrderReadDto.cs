using Kernel;
using SharedKernel;

namespace Application.Orders.Queries.Dtos;

public record OrderReadDto(Guid OrderId, string ExternalUserId, Money Total, List<LineItemReadDto> Lines);