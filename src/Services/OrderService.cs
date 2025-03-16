using ProvaPub.Exceptions;
using ProvaPub.Models;
using ProvaPub.Repository.Interfaces;
using ProvaPub.Services.Interfaces;
using ProvaPub.Strategy.Interfaces;
using System.Runtime.InteropServices;

namespace ProvaPub.Services;

public class OrderService : IOrderService
{
    private readonly ILogger<OrderService> _logger;
    private readonly ICustomerRepository _customerRepository;
    private readonly Dictionary<string, IPaymentStrategy> _paymentStrategies;

    public OrderService(ILogger<OrderService> logger, ICustomerRepository customerRepository, IEnumerable<IPaymentStrategy> paymentStrategies)
    {
        _logger = logger;
        _customerRepository = customerRepository;
        _paymentStrategies = InitializePaymentStrategies(paymentStrategies);
    }

    private Dictionary<string, IPaymentStrategy> InitializePaymentStrategies(IEnumerable<IPaymentStrategy> paymentStrategies)
    {
        return paymentStrategies.ToDictionary(
            paymentStrategy => paymentStrategy.GetType().Name.Replace("PaymentStrategy", string.Empty),
            paymentStrategy => paymentStrategy,
            StringComparer.OrdinalIgnoreCase
        );
    }

    public async Task<Order> PayOrderAsync(string paymentMethod, decimal paymentValue, int customerId)
    {
        if (!_paymentStrategies.TryGetValue(paymentMethod, out var paymentStrategy))
            throw new InvalidOperationException("Método de pagamento inválido.");

        var customer = await _customerRepository.GetById(customerId)
            ?? throw new InvalidOperationException("Cliente não encontrado na base de dados.");

        try
        {
            var order = await paymentStrategy.PayOrderAsync(paymentValue, customerId);

            _logger.LogInformation("Pedido {OrderId} pago com sucesso via {PaymentMethod}", order.Id, paymentMethod);

            order.OrderDate = ConvertToBrazilianTime(order.OrderDate);
            order.Customer = customer;

            return order;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Erro ao processar pagamento via {PaymentMethod}: {MessageException}", paymentMethod, exception.Message);

            throw new PaymentProcessingException("Erro ao processar pagamento. Tente novamente mais tarde.");
        }
    }

    private static DateTime ConvertToBrazilianTime(DateTime utcDateTime)
    {
        string brazilTimeZoneId = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? "E. South America Standard Time"
            : "America/Sao_Paulo";

        return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, TimeZoneInfo.FindSystemTimeZoneById(brazilTimeZoneId));
    }
}
