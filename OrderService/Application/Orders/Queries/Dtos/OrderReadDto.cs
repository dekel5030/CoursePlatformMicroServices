using SharedKernel;
using SharedKernel.Customers;
using SharedKernel.Orders;

namespace Application.Orders.Queries.Dtos;

public record OrderReadDto(OrderId Id, CustomerId CustomerId, Money TotalMoney, List<LineItemReadDto> Lines);