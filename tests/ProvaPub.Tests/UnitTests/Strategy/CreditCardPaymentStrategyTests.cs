using Moq;
using ProvaPub.Models;
using ProvaPub.Repository.Interfaces;
using ProvaPub.Strategy;

namespace ProvaPub.Tests.UnitTests.Strategy;

public class CreditCardPaymentStrategyTests
{
    private readonly CreditCardPaymentStrategy _paymentStrategy;
    private readonly Mock<IOrderRepository> _orderRepositoryMock;

    public CreditCardPaymentStrategyTests()
    {
        _orderRepositoryMock = new Mock<IOrderRepository>();
        _paymentStrategy = new CreditCardPaymentStrategy(_orderRepositoryMock.Object);
    }

    [Fact(DisplayName = "Deve criar um pedido corretamente ao processar pagamento via cartão de crédito")]
    public async Task PayOrderAsync_ShouldCreateOrderSuccessfully()
    {
        int customerId = 1;
        decimal paymentValue = 150.00m;
        var order = new Order
        {
            CustomerId = customerId,
            Value = paymentValue,
            OrderDate = DateTime.UtcNow
        };

        _orderRepositoryMock.Setup(orderRepository => orderRepository.SaveOrderAsync(It.IsAny<Order>()))
                            .Returns(Task.CompletedTask)
                            .Verifiable();

        var result = await _paymentStrategy.PayOrderAsync(paymentValue, customerId);

        Assert.NotNull(result);
        Assert.Equal(customerId, result.CustomerId);
        Assert.Equal(paymentValue, result.Value);
        Assert.True(result.OrderDate <= DateTime.UtcNow);

        _orderRepositoryMock.Verify(orderRepository => orderRepository.SaveOrderAsync(It.IsAny<Order>()), Times.Once);
    }

    [Fact(DisplayName = "Deve lançar exceção ao falhar no salvamento do pedido")]
    public async Task PayOrderAsync_ShouldThrowException_WhenOrderSaveFails()
    {
        int customerId = 1;
        decimal paymentValue = 200.00m;
        var exception = new Exception("Erro ao salvar pedido");

        _orderRepositoryMock.Setup(orderRepository => orderRepository.SaveOrderAsync(It.IsAny<Order>()))
                            .ThrowsAsync(exception);

        var thrownException = await Assert.ThrowsAsync<Exception>(
            () => _paymentStrategy.PayOrderAsync(paymentValue, customerId));

        Assert.Equal("Erro ao salvar pedido", thrownException.Message);
        _orderRepositoryMock.Verify(orderRepository => orderRepository.SaveOrderAsync(It.IsAny<Order>()), Times.Once);
    }
}