using SharedKernel;

namespace Application.Orders.Queries.Dtos;

public record OrderReadDto(Guid Id, Guid UserId, Money Total, List<LineItemReadDto> Lines);