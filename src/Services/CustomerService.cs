using Microsoft.EntityFrameworkCore;
using ProvaPub.Infra;
using ProvaPub.Models;
using ProvaPub.Repository.Interfaces;
using ProvaPub.Services.Interfaces;

namespace ProvaPub.Services;

public class CustomerService : ICustomerService
{
    private readonly TestDbContext _ctx;
    private readonly ILogger<CustomerService> _logger;
    private readonly ICustomerRepository _customerRepository;

    public CustomerService(TestDbContext ctx, ILogger<CustomerService> logger, ICustomerRepository customerRepository)
    {
        _ctx = ctx;
        _logger = logger;
        _customerRepository = customerRepository;
    }

    public async Task<PaginatedList<Customer>> ListCustomersAsync(int page)
    {
        try
        {
            return await _customerRepository.ListItemsAsync(page);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar clientes para a página {Page}", page);

            throw new Exception("Erro ao buscar os clientes. Tente novamente mais tarde.");
        }
    }

    // TO DO: refatorar método.
    public async Task<bool> CanPurchase(int customerId, decimal purchaseValue)
    {
        if (customerId <= 0) throw new ArgumentOutOfRangeException(nameof(customerId));

        if (purchaseValue <= 0) throw new ArgumentOutOfRangeException(nameof(purchaseValue));

        //Business Rule: Non registered Customers cannot purchase
        var customer = await _ctx.Customers.FindAsync(customerId);
        if (customer == null) throw new InvalidOperationException($"Customer Id {customerId} does not exists");

        //Business Rule: A customer can purchase only a single time per month
        var baseDate = DateTime.UtcNow.AddMonths(-1);
        var ordersInThisMonth = await _ctx.Orders.CountAsync(s => s.CustomerId == customerId && s.OrderDate >= baseDate);
        if (ordersInThisMonth > 0)
            return false;

        //Business Rule: A customer that never bought before can make a first purchase of maximum 100,00
        var haveBoughtBefore = await _ctx.Customers.CountAsync(s => s.Id == customerId && s.Orders.Any());
        if (haveBoughtBefore == 0 && purchaseValue > 100)
            return false;

        //Business Rule: A customer can purchases only during business hours and working days
        if (DateTime.UtcNow.Hour < 8 || DateTime.UtcNow.Hour > 18 || DateTime.UtcNow.DayOfWeek == DayOfWeek.Saturday || DateTime.UtcNow.DayOfWeek == DayOfWeek.Sunday)
            return false;


        return true;
    }

}
