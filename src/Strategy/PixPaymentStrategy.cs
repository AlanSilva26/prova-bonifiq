using ProvaPub.Models;
using ProvaPub.Repository.Interfaces;
using ProvaPub.Strategy.Interfaces;

namespace ProvaPub.Strategy;

public class PixPaymentStrategy : IPaymentStrategy
{
    private readonly IOrderRepository _orderRepository;

    public PixPaymentStrategy(IOrderRepository orderRepository) => _orderRepository = orderRepository;

    public async Task<Order> PayOrderAsync(decimal paymentValue, int customerId)
    {
        var order = new Order
        {
            CustomerId = customerId,
            Value = paymentValue,
            OrderDate = DateTime.UtcNow
        };

        await _orderRepository.SaveOrderAsync(order);

        return order;
    }
}
