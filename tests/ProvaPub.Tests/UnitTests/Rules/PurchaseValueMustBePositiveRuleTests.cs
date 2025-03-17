using ProvaPub.Models;
using ProvaPub.Services.Rules;

namespace ProvaPub.Tests.UnitTests.Rules;

public class PurchaseValueMustBePositiveRuleTests
{
    private readonly PurchaseValueMustBePositiveRule _rule = new();

    [Fact(DisplayName = "Deve permitir compra se valor for positivo")]
    public async Task ValidateAsync_ShouldReturnTrue_WhenValueIsPositive()
    {
        var customer = new Customer { Id = 1 };

        var result = await _rule.ValidateAsync(customer, 10, DateTime.UtcNow);

        Assert.True(result);
    }

    [Fact(DisplayName = "Deve lançar exceção se valor for zero ou negativo")]
    public async Task ValidateAsync_ShouldThrowException_WhenValueIsNotPositive()
    {
        var customer = new Customer { Id = 1 };

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() =>
            _rule.ValidateAsync(customer, 0, DateTime.UtcNow));

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() =>
            _rule.ValidateAsync(customer, -1, DateTime.UtcNow));
    }
}
