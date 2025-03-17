using Microsoft.EntityFrameworkCore;
using ProvaPub.Infra;
using ProvaPub.Models;
using ProvaPub.Repository.Interfaces;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace ProvaPub.Repository;

[ExcludeFromCodeCoverage]
public class OrderRepository : IOrderRepository
{
    private readonly TestDbContext _context;

    public OrderRepository(TestDbContext context) => _context = context;

    public async Task<int> CountAsync(Expression<Func<Order, bool>> expression)
    {
        return await _context.Orders.CountAsync(expression);
    }

    public async Task SaveOrderAsync(Order order)
    {
        await _context.Orders.AddAsync(order);
        await _context.SaveChangesAsync();
    }
}
