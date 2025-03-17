using ProvaPub.Models;

namespace ProvaPub.Services.Interfaces;

public interface ICanPurchaseRule
{
    Task<bool> ValidateAsync(Customer customer, decimal purchaseValue, DateTime currentDateTime);
}
