using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Orders.Queries.Dtos;
using Kernel;
using Microsoft.EntityFrameworkCore;

namespace Application.Orders.Queries.GetOrders;

public sealed class GetOrdersQueryHandler
    : IQueryHandler<GetOrdersQuery, PagedResponse<OrderReadDto>>
{
    private readonly IApplicationDbContext _dbContext;

    public GetOrdersQueryHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<PagedResponse<OrderReadDto>>> Handle(
        GetOrdersQuery request,
        CancellationToken cancellationToken = default)
    {
        var baseQuery = _dbContext.Orders.AsNoTracking();
        var totalItems = await baseQuery.CountAsync(cancellationToken);

        PaginationParams pageParams = request.Pagination;
        int pageNumber = pageParams.PageNumber ?? 1;
        int pageSize = Math.Clamp(pageParams.PageSize ?? 10, 1, 100);

        var items = await baseQuery
            .OrderByDescending(o => o.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(o => new OrderReadDto(
                o.Id.Value,
                o.CustomerId.Value,
                o.TotalPrice,
                o.Lines.Select(li => new LineItemReadDto(
                    li.ProductId.Value,
                    li.Quantity,
                    li.Name,
                    li.UnitPrice,
                    li.TotalPrice
                )).ToList()
            ))
            .ToListAsync(cancellationToken);

        var paged = new PagedResponse<OrderReadDto>(
            Items: items,
            TotalItems: totalItems,
            PageNumber: pageNumber,
            PageSize: pageSize
        );

        return Result.Success(paged);
    }
}
