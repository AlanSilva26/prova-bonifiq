using ProvaPub.Infra;
using ProvaPub.Models;
using ProvaPub.Repository.Interfaces;

namespace ProvaPub.Repository;

public class CustomerRepository : PaginatedRepository<Customer>, ICustomerRepository
{
    public CustomerRepository(TestDbContext context) : base(context) { }
}
