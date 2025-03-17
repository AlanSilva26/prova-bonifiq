using ProvaPub.Helpers.Interfaces;
using ProvaPub.Models;
using ProvaPub.Repository.Interfaces;
using ProvaPub.Services.Interfaces;

namespace ProvaPub.Services;

public class CustomerService : ICustomerService
{
    private readonly ILogger<CustomerService> _logger;
    private readonly ICustomerRepository _customerRepository;
    private readonly IEnumerable<ICanPurchaseRule> _purchaseRules;
    private readonly IDateTimeProvider _dateTimeProvider;

    public CustomerService(
        ILogger<CustomerService> logger,
        ICustomerRepository customerRepository,
        IEnumerable<ICanPurchaseRule> purchaseRules,
        IDateTimeProvider dateTimeProvider
    )
    {
        _logger = logger;
        _customerRepository = customerRepository;
        _purchaseRules = purchaseRules;
        _dateTimeProvider = dateTimeProvider;
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

    public async Task<bool> CanPurchase(int customerId, decimal purchaseValue)
    {
        var customer = await _customerRepository.GetById(customerId);

        var currentDateTime = _dateTimeProvider.UtcNow;

        foreach (var rule in _purchaseRules)
        {
            var isValid = await rule.ValidateAsync(customer, purchaseValue, currentDateTime);

            if (!isValid)
                return false;
        }

        return true;
    }
}
