using ProvaPub.Models;
using ProvaPub.Services.Rules;

namespace ProvaPub.Tests.UnitTests.Rules;

public class BusinessHoursPurchaseRuleTests
{
    private readonly BusinessHoursPurchaseRule _rule = new();

    [Theory(DisplayName = "Deve permitir compra dentro do horário comercial")]
    [InlineData(8)]
    [InlineData(12)]
    [InlineData(18)]
    public async Task ValidateAsync_ShouldReturnTrue_WithinBusinessHours(int hour)
    {
        var customer = new Customer { Id = 1 };
        var dateTime = new DateTime(2025, 3, 17, hour, 0, 0);

        var result = await _rule.ValidateAsync(customer, 100, dateTime);

        Assert.True(result);
    }

    [Theory(DisplayName = "Deve bloquear compra fora do horário comercial")]
    [InlineData(7)]
    [InlineData(19)]
    [InlineData(9, DayOfWeek.Saturday)]
    [InlineData(10, DayOfWeek.Sunday)]
    public async Task ValidateAsync_ShouldReturnFalse_OutsideBusinessHours(int hour, DayOfWeek day = DayOfWeek.Monday)
    {
        var customer = new Customer { Id = 1 };
        var dateTime = new DateTime(2025, 3, 17, hour, 0, 0).AddDays((int)day - (int)DayOfWeek.Monday);

        var result = await _rule.ValidateAsync(customer, 100, dateTime);

        Assert.False(result);
    }
}
