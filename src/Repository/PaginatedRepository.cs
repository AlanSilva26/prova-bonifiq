using Microsoft.EntityFrameworkCore;
using ProvaPub.Infra;
using ProvaPub.Models;
using ProvaPub.Repository.Interfaces;

namespace ProvaPub.Repository
{
    public abstract class PaginatedRepository<T> : IPaginatedRepository<T> where T : class
    {
        private readonly TestDbContext _context;
        private const int _pageSize = 10;

        protected PaginatedRepository(TestDbContext context) => _context = context;

        public async Task<PaginatedList<T>> ListItemsAsync(int page)
        {
            if (page < 1)
                page = 1;

            int totalCount = await _context.Set<T>().CountAsync();

            var items = await _context.Set<T>()
                                         .Skip((page - 1) * _pageSize)
                                         .Take(_pageSize)
                                         .ToListAsync();

            return new PaginatedList<T>(items, totalCount, page, _pageSize);
        }
    }
}
