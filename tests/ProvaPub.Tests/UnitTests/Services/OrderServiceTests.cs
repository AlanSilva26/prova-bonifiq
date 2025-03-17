using AutoFixture;
using Microsoft.Extensions.Logging;
using Moq;
using ProvaPub.Exceptions;
using ProvaPub.Models;
using ProvaPub.Repository.Interfaces;
using ProvaPub.Services;
using ProvaPub.Strategy;
using ProvaPub.Strategy.Interfaces;
using ProvaPub.Tests.Helpers;
using System.Reflection;
using System.Runtime.InteropServices;

namespace ProvaPub.Tests.UnitTests.Services;

public class OrderServiceTests
{
    private readonly OrderService _orderService;
    private readonly Mock<ICustomerRepository> _customerRepositoryMock;
    private readonly Mock<IOrderRepository> _orderRepositoryMock;
    private readonly Mock<ILogger<OrderService>> _loggerMock;
    private readonly Fixture _fixture;

    public OrderServiceTests()
    {
        _fixture = new Fixture();
        _fixture.Behaviors.OfType<ThrowingRecursionBehavior>()
                          .ToList()
                          .ForEach(behavior => _fixture.Behaviors.Remove(behavior));

        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        _customerRepositoryMock = new Mock<ICustomerRepository>();
        _orderRepositoryMock = new Mock<IOrderRepository>();
        _loggerMock = new Mock<ILogger<OrderService>>();

        var paymentStrategies = new List<IPaymentStrategy>
        {
            new PixPaymentStrategy(_orderRepositoryMock.Object),
            new PayPalPaymentStrategy(_orderRepositoryMock.Object),
            new CreditCardPaymentStrategy(_orderRepositoryMock.Object)
        };

        _orderService = new OrderService(
            _loggerMock.Object,
            _customerRepositoryMock.Object,
            paymentStrategies
        );
    }

    [Fact(DisplayName = "Deve processar pagamento com sucesso via Pix")]
    public async Task PayOrderAsync_ShouldProcessPaymentSuccessfully()
    {
        int customerId = 1;
        decimal value = 100;
        string paymentMethod = "Pix";

        var customer = _fixture.Build<Customer>()
                               .With(c => c.Id, customerId)
                               .Create();

        var order = new Order
        {
            Id = 123,
            CustomerId = customerId,
            Value = value,
            OrderDate = DateTime.UtcNow
        };

        _customerRepositoryMock.Setup(customerRepository => customerRepository.GetById(customerId))
                               .ReturnsAsync(customer);

        _orderRepositoryMock.Setup(orderRepository => orderRepository.SaveOrderAsync(It.IsAny<Order>()))
                            .Returns(Task.CompletedTask);

        var result = await _orderService.PayOrderAsync(paymentMethod, value, customerId);

        Assert.NotNull(result);
        Assert.Equal(customerId, result.CustomerId);
        Assert.Equal(value, result.Value);

        _loggerMock.VerifyLogging(
            expectedMessage: $"Pedido {result.Id} pago com sucesso via {paymentMethod}",
            expectedLogLevel: LogLevel.Information
        );

        _customerRepositoryMock.Verify(
            expression: customerRepository => customerRepository.GetById(customerId),
            times: Times.Once
        );

        _orderRepositoryMock.Verify(
            expression: orderRepository => orderRepository.SaveOrderAsync(It.IsAny<Order>()),
            times: Times.Once
        );
    }

    [Fact(DisplayName = "Deve lançar erro quando método de pagamento não existe")]
    public async Task PayOrderAsync_ShouldThrowException_WhenPaymentMethodIsInvalid()
    {
        int customerId = 1;
        decimal value = 200;
        string paymentMethod = "Bitcoin";

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _orderService.PayOrderAsync(paymentMethod, value, customerId));

        Assert.Equal("Método de pagamento inválido.", exception.Message);
    }

    [Fact(DisplayName = "Deve lançar erro quando cliente não existe")]
    public async Task PayOrderAsync_ShouldThrowException_WhenCustomerDoesNotExist()
    {
        int customerId = 999;
        decimal value = 200;
        string paymentMethod = "Pix";

        _customerRepositoryMock.Setup(customerRepository => customerRepository.GetById(customerId))
                               .ReturnsAsync((Customer)null!);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _orderService.PayOrderAsync(paymentMethod, value, customerId)
        );

        Assert.Equal("Cliente não encontrado na base de dados.", exception.Message);

        _orderRepositoryMock.Verify(
            expression: orderRepository => orderRepository.SaveOrderAsync(It.IsAny<Order>()),
            times: Times.Never
        );
    }

    [Fact(DisplayName = "Deve lançar erro ao falhar processamento de pagamento")]
    public async Task PayOrderAsync_ShouldThrowException_WhenPaymentProcessingFails()
    {
        int customerId = 3;
        decimal value = 150;
        string paymentMethod = "CreditCard";

        var customer = _fixture.Build<Customer>()
                               .With(c => c.Id, customerId)
                               .Create();

        _customerRepositoryMock.Setup(customerRepository => customerRepository.GetById(customerId))
                               .ReturnsAsync(customer);

        var exception = new Exception("Erro no processamento do pagamento");

        _orderRepositoryMock.Setup(orderRepository => orderRepository.SaveOrderAsync(It.IsAny<Order>()))
                            .ThrowsAsync(exception);

        var resultException = await Assert.ThrowsAsync<PaymentProcessingException>(() =>
            _orderService.PayOrderAsync(paymentMethod, value, customerId)
        );

        Assert.Equal("Erro ao processar pagamento. Tente novamente mais tarde.", resultException.Message);

        _loggerMock.VerifyLogging(
            expectedMessage: $"Erro ao processar pagamento via {paymentMethod}: {exception.Message}",
            expectedLogLevel: LogLevel.Error
        );

        _orderRepositoryMock.Verify(
            expression: orderRepository => orderRepository.SaveOrderAsync(It.IsAny<Order>()),
            times: Times.Once
        );
    }

    [Fact(DisplayName = "Deve converter Data para UTC-3 corretamente")]
    public void ConvertToBrazilianTime_ShouldConvertUtcTimeToBRT()
    {
        DateTime utcTime = new DateTime(2024, 3, 16, 12, 0, 0, DateTimeKind.Utc);
        DateTime expectedBRT = utcTime.AddHours(-3);

        MethodInfo method = typeof(OrderService).GetMethod("ConvertToBrazilianTime", BindingFlags.NonPublic | BindingFlags.Static)!;

        var result = (DateTime)method.Invoke(null, new object[] { utcTime })!;

        Assert.Equal(expectedBRT, result);
    }

    [Fact(DisplayName = "Deve lançar erro ao converter Data se Timezone for inválido")]
    public void ConvertToBrazilianTime_ShouldThrowException_WhenTimezoneIsInvalid()
    {
        string invalidTimezone = "Invalid/Timezone";

        var method = typeof(OrderService).GetMethod("ConvertToBrazilianTime", BindingFlags.NonPublic | BindingFlags.Static)!;

        var exception = Record.Exception(() => method.Invoke(null, new object[] { invalidTimezone }));

        Assert.NotNull(exception);
    }

    [Theory(DisplayName = "Deve converter horário UTC para horário brasileiro corretamente")]
    [InlineData(true, "E. South America Standard Time")]
    [InlineData(false, "America/Sao_Paulo")]
    public void ConvertToBrazilianTime_ShouldUseCorrectTimeZone(bool isWindows, string expectedTimeZone)
    {
        DateTime utcTime = new DateTime(2025, 3, 16, 12, 0, 0, DateTimeKind.Utc);

        var runtimeInfoMock = new Mock<IRuntimeInformation>();
        runtimeInfoMock.Setup(r => r.IsOSPlatform(OSPlatform.Windows)).Returns(isWindows);

        MethodInfo method = typeof(OrderService)
            .GetMethod("ConvertToBrazilianTime", BindingFlags.NonPublic | BindingFlags.Static)!;

        var result = (DateTime)method.Invoke(null, new object[] { utcTime })!;

        DateTime expectedBRT = utcTime.AddHours(-3);
        Assert.Equal(expectedBRT, result);
    }
}

public interface IRuntimeInformation
{
    bool IsOSPlatform(OSPlatform os);
}
