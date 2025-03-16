using ProvaPub.Infra;
using ProvaPub.Models;
using ProvaPub.Repository.Interfaces;

namespace ProvaPub.Repository;

public class OrderRepository : IOrderRepository
{
    private readonly TestDbContext _context;

    public OrderRepository(TestDbContext context) => _context = context;

    public async Task SaveOrderAsync(Order order)
    {
        await _context.Orders.AddAsync(order);
        await _context.SaveChangesAsync();
    }
}
