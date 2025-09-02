using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Orders.Queries.Dtos;
using Domain.Orders.Errors;
using Kernel;
using Microsoft.EntityFrameworkCore;

namespace Application.Orders.Queries.GetById;

public sealed class GetOrderByIdQueryHandler
    : IQueryHandler<GetOrderByIdQuery, OrderReadDto>
{
    private readonly IApplicationDbContext _dbContext;

    public GetOrderByIdQueryHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<OrderReadDto>> Handle(GetOrderByIdQuery query, CancellationToken cancellationToken)
    {
        var orderReadDto = await _dbContext.Orders
            .AsNoTracking()
            .Where(o => o.Id == query.OrderId)
            .Select(o => new OrderReadDto(
                o.Id.Value,
                o.ExternalUserId.Value,
                o.TotalPrice,
                o.Lines.Select(li => new LineItemReadDto(
                    li.ProductId.Value,
                    li.Quantity,
                    li.Name,
                    li.UnitPrice,
                    li.TotalPrice
                )).ToList()
            ))
            .FirstOrDefaultAsync(cancellationToken);

        if (orderReadDto is null)
        {
            return Result.Failure<OrderReadDto>(OrderErrors.NotFound);
        }

        return Result.Success(orderReadDto);
    }
}
