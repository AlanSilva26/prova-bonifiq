using ProvaPub.Models;
using ProvaPub.Repository.Interfaces;
using ProvaPub.Services.Interfaces;

namespace ProvaPub.Services.Rules;

public class FirstPurchaseLimitRule : ICanPurchaseRule
{
    private readonly ICustomerRepository _customerRepository;

    public FirstPurchaseLimitRule(ICustomerRepository customerRepository) => _customerRepository = customerRepository;

    public async Task<bool> ValidateAsync(Customer customer, decimal purchaseValue, DateTime currentDateTime)
    {
        var haveBoughtBefore = await _customerRepository.CountAsync(c => c.Id == customer.Id && c.Orders.Any());

        return haveBoughtBefore > 0 || purchaseValue <= 100;
    }
}
