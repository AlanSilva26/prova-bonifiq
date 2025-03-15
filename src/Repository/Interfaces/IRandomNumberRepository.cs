using ProvaPub.Models;

namespace ProvaPub.Repository.Interfaces;

public interface IRandomNumberRepository
{
    Task<bool> ExistsAsync(int number);

    Task SaveAsync(RandomNumber number);
}
