using SharedKernel.Orders;

namespace Application.Orders.Queries.Dtos;

public class OrderReadDto
{
    public OrderReadDto(OrderId id, object value, string currency, object total, List<LineItemReadDto> lines)
    {
        Id = id;
        Value = value;
        Currency = currency;
        Total = total;
        Lines = lines;
    }

    public OrderId Id { get; }
    public object Value { get; }
    public string Currency { get; }
    public object Total { get; }
    public List<LineItemReadDto> Lines { get; }
}