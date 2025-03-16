﻿using ProvaPub.Models;
using ProvaPub.Repository.Interfaces;
using ProvaPub.Strategy.Interfaces;

namespace ProvaPub.Strategy;

public class CreditCardPaymentStrategy : IPaymentStrategy
{
    private readonly IOrderRepository _orderRepository;

    public CreditCardPaymentStrategy(IOrderRepository orderRepository) => _orderRepository = orderRepository;

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
