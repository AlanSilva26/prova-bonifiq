using ProvaPub.Models;
using System.Linq.Expressions;

namespace ProvaPub.Repository.Interfaces;

public interface IOrderRepository
{
    Task<int> CountAsync(Expression<Func<Order, bool>> expression);

    Task SaveOrderAsync(Order order);
}
