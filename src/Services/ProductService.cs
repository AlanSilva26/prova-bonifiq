using ProvaPub.Models;
using ProvaPub.Repository.Interfaces;
using ProvaPub.Services.Interfaces;

namespace ProvaPub.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly ILogger<ProductService> _logger;

    public ProductService(IProductRepository productRepository, ILogger<ProductService> logger)
    {
        _productRepository = productRepository;
        _logger = logger;
    }

    public async Task<PaginatedList<Product>> ListProductsAsync(int page)
    {
        try
        {
            return await _productRepository.ListItemsAsync(page);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar produtos para a página {Page}", page);

            throw new Exception("Erro ao buscar os produtos. Tente novamente mais tarde.");
        }
    }
}
