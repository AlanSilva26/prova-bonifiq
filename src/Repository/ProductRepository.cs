using ProvaPub.Infra;
using ProvaPub.Models;
using ProvaPub.Repository.Interfaces;

namespace ProvaPub.Repository;

public class ProductRepository : PaginatedRepository<Product>, IProductRepository
{
    public ProductRepository(TestDbContext context) : base(context) { }
}
