using ProvaPub.Models;

namespace ProvaPub.Strategy.Interfaces;

public interface IPaymentStrategy
{
    Task<Order> PayOrderAsync(decimal paymentValue, int customerId);
}
