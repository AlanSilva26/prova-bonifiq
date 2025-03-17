using ProvaPub.Models;
using ProvaPub.Repository.Interfaces;
using ProvaPub.Services.Interfaces;

namespace ProvaPub.Services.Rules;

public class CustomerMustWait30DaysRule : ICanPurchaseRule
{
    private readonly IOrderRepository _orderRepository;

    public CustomerMustWait30DaysRule(IOrderRepository orderRepository) => _orderRepository = orderRepository;

    public async Task<bool> ValidateAsync(Customer customer, decimal purchaseValue, DateTime currentDateTime)
    {
        var baseDate = currentDateTime.AddMonths(-1);

        var ordersInThisMonth = await _orderRepository.CountAsync(order => order.CustomerId == customer.Id && order.OrderDate >= baseDate);

        return ordersInThisMonth == 0;
    }
}
