using SharedKernel;

namespace Application.Orders.Queries.Dtos;

public record OrderReadDto(Guid Id, Guid CustomerId, Money Total, List<LineItemReadDto> Lines);