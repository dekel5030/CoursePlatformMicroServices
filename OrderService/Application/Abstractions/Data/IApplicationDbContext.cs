using Domain.Orders;
using Microsoft.EntityFrameworkCore;

namespace Application.Abstractions.Data;

public interface IApplicationDbContext
{
    DbSet<Order> Orders { get; }
    DbSet<LineItem> LineItems { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
