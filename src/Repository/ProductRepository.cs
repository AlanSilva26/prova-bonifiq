using ProvaPub.Infra;
using ProvaPub.Models;
using ProvaPub.Repository.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace ProvaPub.Repository;

[ExcludeFromCodeCoverage]
public class ProductRepository : PaginatedRepository<Product>, IProductRepository
{
    public ProductRepository(TestDbContext context) : base(context) { }
}
