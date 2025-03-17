using ProvaPub.Models;

namespace ProvaPub.Repository.Interfaces;

public interface IPaginatedRepository<T> where T : class
{
    Task<PaginatedList<T>> ListItemsAsync(int page);
}
