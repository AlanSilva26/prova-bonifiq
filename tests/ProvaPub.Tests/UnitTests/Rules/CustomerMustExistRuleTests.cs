using ProvaPub.Models;
using ProvaPub.Services.Rules;

namespace ProvaPub.Tests.UnitTests.Rules;

public class CustomerMustExistRuleTests
{
    private readonly CustomerMustExistRule _rule = new();

    [Fact(DisplayName = "Deve permitir compra se o cliente existir")]
    public async Task ValidateAsync_ShouldReturnTrue_WhenCustomerExists()
    {
        var customer = new Customer { Id = 1 };

        var result = await _rule.ValidateAsync(customer, 100, DateTime.UtcNow);

        Assert.True(result);
    }

    [Fact(DisplayName = "Deve lançar exceção se o cliente não existir")]
    public async Task ValidateAsync_ShouldThrowException_WhenCustomerDoesNotExist()
    {
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _rule.ValidateAsync(null!, 100, DateTime.UtcNow)
        );
    }
}
