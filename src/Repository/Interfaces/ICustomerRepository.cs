using ProvaPub.Models;

namespace ProvaPub.Repository.Interfaces
{
    public interface ICustomerRepository : IPaginatedRepository<Customer>
    {
        Task<Customer?> GetById(int id);

        // CanPurchase
    }
}
