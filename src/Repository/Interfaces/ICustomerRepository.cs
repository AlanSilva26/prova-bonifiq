using ProvaPub.Models;
using System.Linq.Expressions;

namespace ProvaPub.Repository.Interfaces
{
    public interface ICustomerRepository : IPaginatedRepository<Customer>
    {
        Task<int> CountAsync(Expression<Func<Customer, bool>> expression);

        Task<Customer?> GetById(int id);

        // CanPurchase
    }
}
