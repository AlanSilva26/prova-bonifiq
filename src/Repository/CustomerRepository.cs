using Microsoft.EntityFrameworkCore;
using ProvaPub.Infra;
using ProvaPub.Models;
using ProvaPub.Repository.Interfaces;

namespace ProvaPub.Repository;

public class CustomerRepository : PaginatedRepository<Customer>, ICustomerRepository
{
    private readonly TestDbContext _context;

    public CustomerRepository(TestDbContext context) : base(context) => _context = context;

    public async Task<Customer?> GetById(int id)
    {
        var customer = await _context.Customers.AsNoTracking()
                                               .FirstOrDefaultAsync(cutomer => cutomer.Id == id);

        return customer;
    }
}
