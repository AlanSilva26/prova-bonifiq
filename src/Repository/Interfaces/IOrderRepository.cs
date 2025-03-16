using ProvaPub.Models;

namespace ProvaPub.Repository.Interfaces;

public interface IOrderRepository
{
    Task SaveOrderAsync(Order order);
}
