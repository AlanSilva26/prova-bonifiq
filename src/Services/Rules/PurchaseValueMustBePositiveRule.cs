using ProvaPub.Models;
using ProvaPub.Services.Interfaces;

namespace ProvaPub.Services.Rules;

public class PurchaseValueMustBePositiveRule : ICanPurchaseRule
{
    public Task<bool> ValidateAsync(Customer customer, decimal purchaseValue, DateTime currentDateTime)
    {
        if (purchaseValue <= 0)
            throw new ArgumentOutOfRangeException(nameof(purchaseValue), "O valor da compra deve ser maior que zero.");

        return Task.FromResult(true);
    }
}
