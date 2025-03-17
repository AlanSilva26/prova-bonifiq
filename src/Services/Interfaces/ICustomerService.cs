using ProvaPub.Models;

namespace ProvaPub.Services.Interfaces;

public interface ICustomerService
{
    Task<PaginatedList<Customer>> ListCustomersAsync(int page);

    Task<bool> CanPurchase(int customerId, decimal purchaseValue);
}
