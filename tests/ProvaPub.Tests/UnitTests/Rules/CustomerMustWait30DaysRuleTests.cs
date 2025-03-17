using Moq;
using ProvaPub.Models;
using ProvaPub.Repository.Interfaces;
using ProvaPub.Services.Rules;
using System.Linq.Expressions;

namespace ProvaPub.Tests.UnitTests.Rules;

public class CustomerMustWait30DaysRuleTests
{
    private readonly Mock<IOrderRepository> _orderRepositoryMock;
    private readonly CustomerMustWait30DaysRule _rule;

    public CustomerMustWait30DaysRuleTests()
    {
        _orderRepositoryMock = new Mock<IOrderRepository>();
        _rule = new CustomerMustWait30DaysRule(_orderRepositoryMock.Object);
    }

    [Fact(DisplayName = "Deve permitir compra se cliente não comprou nos últimos 30 dias")]
    public async Task ValidateAsync_ShouldReturnTrue_WhenCustomerHasNoRecentOrders()
    {
        var customer = new Customer { Id = 1 };
        _orderRepositoryMock.Setup(repo => repo.CountAsync(It.IsAny<Expression<Func<Order, bool>>>()))
                            .ReturnsAsync(0);

        var result = await _rule.ValidateAsync(customer, 100, DateTime.UtcNow);

        Assert.True(result);
    }

    [Fact(DisplayName = "Deve bloquear compra se cliente comprou nos últimos 30 dias")]
    public async Task ValidateAsync_ShouldReturnFalse_WhenCustomerHasRecentOrders()
    {
        var customer = new Customer { Id = 1 };
        _orderRepositoryMock.Setup(repo => repo.CountAsync(It.IsAny<Expression<Func<Order, bool>>>()))
                            .ReturnsAsync(1);

        var result = await _rule.ValidateAsync(customer, 100, DateTime.UtcNow);

        Assert.False(result);
    }
}
