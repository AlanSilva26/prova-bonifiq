using Microsoft.Extensions.Logging;
using Moq;
using ProvaPub.Models;
using ProvaPub.Repository.Interfaces;
using ProvaPub.Services;
using ProvaPub.Tests.Helpers;

namespace ProvaPub.Tests.UnitTests.Services
{
    public class CustomerServiceTests
    {
        private readonly CustomerService _customerService;
        private readonly Mock<ICustomerRepository> _customerRepositoryMock;
        private readonly Mock<ILogger<CustomerService>> _loggerMock;

        public CustomerServiceTests()
        {
            _customerRepositoryMock = new Mock<ICustomerRepository>();
            _loggerMock = new Mock<ILogger<CustomerService>>();

            _customerService = new CustomerService(ctx: null!, _loggerMock.Object, _customerRepositoryMock.Object);
        }

        [Fact(DisplayName = "Deve retornar uma lista paginada de clientes")]
        public async Task ListCustomersAsync_ShouldReturnPaginatedCustomers()
        {
            int page = 1;
            int pageSize = 10;
            var customers = new List<Customer>
            {
                new() { Id = 1, Name = "Cliente 1" },
                new() { Id = 2, Name = "Cliente 2" },
            };
            var paginatedList = new PaginatedList<Customer>(customers, 2, page, pageSize);

            _customerRepositoryMock.Setup(customerRepository => customerRepository.ListItemsAsync(page))
                                   .ReturnsAsync(paginatedList);

            var result = await _customerService.ListCustomersAsync(page);

            Assert.NotNull(result);
            Assert.Equal(2, result.Items.Count);
            Assert.Equal("Cliente 1", result.Items.First().Name);
            Assert.Equal("Cliente 2", result.Items.Last().Name);

            _customerRepositoryMock.Verify(
                expression: customerRepository => customerRepository.ListItemsAsync(page),
                times: Times.Once
            );
        }

        [Fact(DisplayName = "Deve registrar erro no log e lançar exceção ao falhar a listagem de clientes")]
        public async Task ListCustomersAsync_ShouldLogErrorAndThrowException_OnFailure()
        {
            int page = 1;
            var exception = new Exception("Erro no banco de dados");

            _customerRepositoryMock.Setup(customerRepository => customerRepository.ListItemsAsync(page))
                                   .ThrowsAsync(exception);

            var exceptionResult = await Assert.ThrowsAsync<Exception>(() => _customerService.ListCustomersAsync(page));

            Assert.Equal("Erro ao buscar os clientes. Tente novamente mais tarde.", exceptionResult.Message);

            _loggerMock.VerifyLogging(
                expectedMessage: "Erro ao listar clientes para a página 1",
                expectedLogLevel: LogLevel.Error
            );

            _customerRepositoryMock.Verify(
                expression: customerRepository => customerRepository.ListItemsAsync(page),
                times: Times.Once
            );
        }
    }
}
