using ProvaPub.Models;

namespace ProvaPub.Services.Interfaces;

public interface IProductService
{
    Task<PaginatedList<Product>> ListProductsAsync(int page);
}
