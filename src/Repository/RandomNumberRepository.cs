using Microsoft.EntityFrameworkCore;
using ProvaPub.Infra;
using ProvaPub.Models;
using ProvaPub.Repository.Interfaces;

namespace ProvaPub.Repository;

public class RandomNumberRepository : IRandomNumberRepository
{
    private readonly TestDbContext _context;

    public RandomNumberRepository(TestDbContext context) => _context = context;

    public async Task<bool> ExistsAsync(int number)
    {
        return await _context.Numbers.AnyAsync(n => n.Number == number);
    }

    public async Task SaveAsync(RandomNumber number)
    {
        _context.Numbers.Add(number);

        await _context.SaveChangesAsync();
    }
}
