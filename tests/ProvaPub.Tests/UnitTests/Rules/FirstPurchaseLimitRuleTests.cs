using Moq;
using ProvaPub.Models;
using ProvaPub.Repository.Interfaces;
using ProvaPub.Services.Rules;
using System.Linq.Expressions;

namespace ProvaPub.Tests.UnitTests.Rules;

public class FirstPurchaseLimitRuleTests
{
    private readonly Mock<ICustomerRepository> _customerRepositoryMock;
    private readonly FirstPurchaseLimitRule _rule;

    public FirstPurchaseLimitRuleTests()
    {
        _customerRepositoryMock = new Mock<ICustomerRepository>();
        _rule = new FirstPurchaseLimitRule(_customerRepositoryMock.Object);
    }

    [Fact(DisplayName = "Deve permitir compra se cliente já comprou antes")]
    public async Task ValidateAsync_ShouldReturnTrue_WhenCustomerHasPreviousOrders()
    {
        var customer = new Customer { Id = 1 };
        _customerRepositoryMock.Setup(repo => repo.CountAsync(It.IsAny<Expression<Func<Customer, bool>>>()))
                               .ReturnsAsync(1);

        var result = await _rule.ValidateAsync(customer, 200, DateTime.UtcNow);

        Assert.True(result);
    }

    [Fact(DisplayName = "Deve permitir compra se cliente nunca comprou e valor for até R$100")]
    public async Task ValidateAsync_ShouldReturnTrue_WhenFirstPurchaseIsWithinLimit()
    {
        var customer = new Customer { Id = 1 };
        _customerRepositoryMock.Setup(repo => repo.CountAsync(It.IsAny<Expression<Func<Customer, bool>>>()))
                               .ReturnsAsync(0);

        var result = await _rule.ValidateAsync(customer, 100, DateTime.UtcNow);

        Assert.True(result);
    }

    [Fact(DisplayName = "Deve retornar false se cliente nunca comprou e valor for maior que R$100")]
    public async Task ValidateAsync_ShouldReturnFalse_WhenFirstPurchaseExceedsLimit()
    {
        var customer = new Customer { Id = 1 };
        _customerRepositoryMock.Setup(repo => repo.CountAsync(It.IsAny<Expression<Func<Customer, bool>>>()))
                               .ReturnsAsync(0);

        var result = await _rule.ValidateAsync(customer, 101, DateTime.UtcNow);

        Assert.False(result);
    }
}
