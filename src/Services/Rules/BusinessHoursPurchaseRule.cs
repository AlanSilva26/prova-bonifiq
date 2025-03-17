using ProvaPub.Models;
using ProvaPub.Services.Interfaces;

namespace ProvaPub.Services.Rules;

public class BusinessHoursPurchaseRule : ICanPurchaseRule
{
    public Task<bool> ValidateAsync(Customer customer, decimal purchaseValue, DateTime currentDateTime)
    {
        bool isValid = currentDateTime.Hour >= 8 && currentDateTime.Hour <= 18 &&
                       currentDateTime.DayOfWeek != DayOfWeek.Saturday &&
                       currentDateTime.DayOfWeek != DayOfWeek.Sunday;

        return Task.FromResult(isValid);
    }
}
