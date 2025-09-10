using Domain.Orders;
using Domain.Products;
using Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Application.Abstractions.Data;
public interface IReadDbContext
{
    DbSet<Order> Orders { get; }
    DbSet<LineItem> LineItems { get; }
    DbSet<User> Users { get; }
    DbSet<Product> Products { get; }
}
