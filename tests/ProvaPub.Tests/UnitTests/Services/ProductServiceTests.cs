using Microsoft.Extensions.Logging;
using Moq;
using ProvaPub.Models;
using ProvaPub.Repository.Interfaces;
using ProvaPub.Services;
using ProvaPub.Tests.Helpers;

namespace ProvaPub.Tests.UnitTests.Services
{
    public class ProductServiceTests
    {
        private readonly ProductService _productService;
        private readonly Mock<IProductRepository> _productRepositoryMock;
        private readonly Mock<ILogger<ProductService>> _loggerMock;

        public ProductServiceTests()
        {
            _productRepositoryMock = new Mock<IProductRepository>();
            _loggerMock = new Mock<ILogger<ProductService>>();

            _productService = new ProductService(_loggerMock.Object, _productRepositoryMock.Object);
        }

        [Fact(DisplayName = "Deve retornar uma lista paginada de produtos")]
        public async Task ListProductsAsync_ShouldReturnPaginatedProducts()
        {
            int page = 1;
            int pageSize = 10;
            var products = new List<Product>
            {
                new() { Id = 1, Name = "Produto 1" },
                new() { Id = 2, Name = "Produto 2" },
            };
            var paginatedList = new PaginatedList<Product>(products, 2, page, pageSize);

            _productRepositoryMock.Setup(productRepository => productRepository.ListItemsAsync(page))
                                  .ReturnsAsync(paginatedList);

            var result = await _productService.ListProductsAsync(page);

            Assert.NotNull(result);
            Assert.Equal(2, result.Items.Count);
            Assert.Equal("Produto 1", result.Items.First().Name);
            Assert.Equal("Produto 2", result.Items.Last().Name);

            _productRepositoryMock.Verify(
                expression: productRepository => productRepository.ListItemsAsync(page),
                times: Times.Once
            );
        }

        [Fact(DisplayName = "Deve registrar erro no log e lançar exceção ao falhar a listagem de produtos")]
        public async Task ListProductsAsync_ShouldLogErrorAndThrowException_OnFailure()
        {
            int page = 1;
            var exception = new Exception("Erro no banco de dados");

            _productRepositoryMock.Setup(productRepository => productRepository.ListItemsAsync(page))
                                  .ThrowsAsync(exception);

            var exceptionResult = await Assert.ThrowsAsync<Exception>(() => _productService.ListProductsAsync(page));

            Assert.Equal("Erro ao buscar os produtos. Tente novamente mais tarde.", exceptionResult.Message);

            _loggerMock.VerifyLogging(
                expectedMessage: "Erro ao listar produtos para a página 1",
                expectedLogLevel: LogLevel.Error
            );

            _productRepositoryMock.Verify(
                expression: productRepository => productRepository.ListItemsAsync(page),
                times: Times.Once
            );
        }
    }
}
