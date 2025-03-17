using Microsoft.Extensions.Logging;
using Moq;
using ProvaPub.Helpers.Interfaces;
using ProvaPub.Models;
using ProvaPub.Repository.Interfaces;
using ProvaPub.Services;
using ProvaPub.Services.Interfaces;
using ProvaPub.Services.Rules;
using ProvaPub.Tests.Helpers;
using System.Linq.Expressions;

namespace ProvaPub.Tests.UnitTests.Services
{
    public class CustomerServiceTests
    {
        private readonly CustomerService _customerService;
        private readonly Mock<ICustomerRepository> _customerRepositoryMock;
        private readonly Mock<IOrderRepository> _orderRepositoryMock;
        private readonly Mock<ILogger<CustomerService>> _loggerMock;
        private readonly Mock<IDateTimeProvider> _dateTimeProviderMock;

        public CustomerServiceTests()
        {
            _loggerMock = new Mock<ILogger<CustomerService>>();
            _customerRepositoryMock = new Mock<ICustomerRepository>();
            _orderRepositoryMock = new Mock<IOrderRepository>();

            var purchaseRules = new List<ICanPurchaseRule>
            {
                new CustomerMustExistRule(),
                new PurchaseValueMustBePositiveRule(),
                new CustomerMustWait30DaysRule(_orderRepositoryMock.Object),
                new FirstPurchaseLimitRule(_customerRepositoryMock.Object),
                new BusinessHoursPurchaseRule()
            };

            _dateTimeProviderMock = new Mock<IDateTimeProvider>();
            _dateTimeProviderMock.Setup(p => p.UtcNow).Returns(new DateTime(2025, 3, 17, 10, 0, 0));

            _customerService = new CustomerService(_loggerMock.Object, _customerRepositoryMock.Object, purchaseRules, _dateTimeProviderMock.Object);
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

        [Fact(DisplayName = "Deve permitir compra para cliente válido sem compras recentes")]
        public async Task CanPurchase_ShouldAllowPurchase_ForValidCustomer()
        {
            int customerId = 1;
            decimal purchaseValue = 100;
            var customer = new Customer { Id = customerId, Orders = new List<Order>() };

            _customerRepositoryMock.Setup(customerRepository => customerRepository.GetById(customerId))
                                   .ReturnsAsync(customer);

            var result = await _customerService.CanPurchase(customerId, purchaseValue);

            Assert.True(result);
        }

        [Fact(DisplayName = "Deve lançar exceção quando cliente não existir")]
        public async Task CanPurchase_ShouldThrowException_WhenCustomerDoesNotExist()
        {
            int customerId = 999;
            decimal purchaseValue = 100;

            _customerRepositoryMock.Setup(customerRepository => customerRepository.GetById(customerId))
                                   .ReturnsAsync((Customer)null!);

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _customerService.CanPurchase(customerId, purchaseValue)
            );

            Assert.Equal("Cliente não encontrado na base de dados.", exception.Message);
        }

        [Fact(DisplayName = "Deve lançar exceção quando valor da compra for zero ou negativo")]
        public async Task CanPurchase_ShouldThrowException_WhenPurchaseValueIsNotPositive()
        {
            int customerId = 1;
            decimal purchaseValue = 0;
            var customer = new Customer { Id = customerId, Orders = new List<Order>() };

            _customerRepositoryMock.Setup(customerRepository => customerRepository.GetById(customerId))
                                   .ReturnsAsync(customer);

            var exception = await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
                () => _customerService.CanPurchase(customerId, purchaseValue)
            );

            Assert.Equal("O valor da compra deve ser maior que zero. (Parameter 'purchaseValue')", exception.Message);
        }

        [Fact(DisplayName = "Deve retornar false quando cliente já realizou compra nos últimos 30 dias")]
        public async Task CanPurchase_ShouldReturnFalse_WhenCustomerHasRecentPurchase()
        {
            int customerId = 1;
            decimal purchaseValue = 100;
            var baseDate = _dateTimeProviderMock.Object.UtcNow.AddMonths(-1);
            var customer = new Customer
            {
                Id = customerId,
                Orders = new List<Order> { new() { OrderDate = _dateTimeProviderMock.Object.UtcNow.AddDays(-10) } }
            };

            _customerRepositoryMock.Setup(customerRepository => customerRepository.GetById(customerId))
                                   .ReturnsAsync(customer);

            _orderRepositoryMock.Setup(orderRepository => orderRepository.CountAsync(It.IsAny<Expression<Func<Order, bool>>>()))
                                .ReturnsAsync(1);

            var result = await _customerService.CanPurchase(customerId, purchaseValue);

            Assert.False(result);
        }

        [Fact(DisplayName = "Deve retornar false quando primeira compra for maior que R$100")]
        public async Task CanPurchase_ShouldReturnFalse_WhenFirstPurchaseExceedsLimit()
        {
            int customerId = 1;
            decimal purchaseValue = 150;
            var customer = new Customer { Id = customerId, Orders = new List<Order>() };

            _customerRepositoryMock.Setup(customerRepository => customerRepository.GetById(customerId))
                                   .ReturnsAsync(customer);

            _customerRepositoryMock.Setup(customerRepository => customerRepository.CountAsync(It.IsAny<Expression<Func<Customer, bool>>>()))
                                   .ReturnsAsync(0);

            var result = await _customerService.CanPurchase(customerId, purchaseValue);

            Assert.False(result);
        }

        [Fact(DisplayName = "Deve retornar false quando compra for fora do horário comercial")]
        public async Task CanPurchase_ShouldReturnFalse_WhenOutsideBusinessHours()
        {
            int customerId = 1;
            decimal purchaseValue = 100;
            var customer = new Customer { Id = customerId, Orders = new List<Order>() };

            _customerRepositoryMock.Setup(customerRepository => customerRepository.GetById(customerId))
                                   .ReturnsAsync(customer);

            _dateTimeProviderMock.Setup(p => p.UtcNow)
                                 .Returns(new DateTime(2025, 3, 17, 20, 0, 0));

            var result = await _customerService.CanPurchase(customerId, purchaseValue);

            Assert.False(result);
        }

        [Fact(DisplayName = "Deve retornar false quando compra for em final de semana")]
        public async Task CanPurchase_ShouldReturnFalse_WhenWeekend()
        {
            int customerId = 1;
            decimal purchaseValue = 100;
            var customer = new Customer { Id = customerId, Orders = new List<Order>() };

            _customerRepositoryMock.Setup(customerRepository => customerRepository.GetById(customerId))
                                   .ReturnsAsync(customer);

            _dateTimeProviderMock.Setup(p => p.UtcNow)
                                 .Returns(new DateTime(2025, 3, 16, 10, 0, 0));

            var result = await _customerService.CanPurchase(customerId, purchaseValue);

            Assert.False(result);
        }
    }
}
