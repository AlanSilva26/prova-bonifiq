using ProvaPub.Models;
using ProvaPub.Services.Interfaces;

namespace ProvaPub.Services.Rules;

public class CustomerMustExistRule : ICanPurchaseRule
{
    public Task<bool> ValidateAsync(Customer customer, decimal purchaseValue, DateTime currentDateTime)
    {
        if (customer is null)
            throw new InvalidOperationException("Cliente não encontrado na base de dados.");

        return Task.FromResult(true);
    }
}
