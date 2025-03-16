using ProvaPub.Models;

namespace ProvaPub.Services.Interfaces;

public interface IOrderService
{
    Task<Order> PayOrderAsync(string paymentMethod, decimal paymentValue, int customerId);
}
