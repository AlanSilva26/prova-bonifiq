using Microsoft.EntityFrameworkCore;
using ProvaPub.Helpers;
using ProvaPub.Helpers.Interfaces;
using ProvaPub.Infra;
using ProvaPub.Repository;
using ProvaPub.Repository.Interfaces;
using ProvaPub.Services;
using ProvaPub.Services.Interfaces;
using ProvaPub.Services.Rules;
using ProvaPub.Strategy;
using ProvaPub.Strategy.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace ProvaPub.IoC;

[ExcludeFromCodeCoverage]
public static class ServiceCollection
{
    public static void AddServiceDefaults(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDatabase(configuration);
        services.AddRepositories();
        services.AddRules();
        services.AddStrategies();
        services.AddServices();
    }

    private static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("ctx");

        services.AddDbContext<TestDbContext>((services, builder) => builder.UseSqlServer(connectionString));

        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IRandomNumberRepository, RandomNumberRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();

        return services;
    }

    private static IServiceCollection AddRules(this IServiceCollection services)
    {
        services.AddScoped<ICanPurchaseRule, CustomerMustExistRule>();
        services.AddScoped<ICanPurchaseRule, PurchaseValueMustBePositiveRule>();
        services.AddScoped<ICanPurchaseRule, CustomerMustWait30DaysRule>();
        services.AddScoped<ICanPurchaseRule, FirstPurchaseLimitRule>();
        services.AddScoped<ICanPurchaseRule, BusinessHoursPurchaseRule>();

        return services;
    }

    private static IServiceCollection AddStrategies(this IServiceCollection services)
    {
        services.AddScoped<IPaymentStrategy, PixPaymentStrategy>();
        services.AddScoped<IPaymentStrategy, CreditCardPaymentStrategy>();
        services.AddScoped<IPaymentStrategy, PayPalPaymentStrategy>();

        return services;
    }

    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IDateTimeProvider, DateTimeProvider>();
        services.AddScoped<INumberGeneratorService, NumberGeneratorService>();
        services.AddScoped<IRandomService, RandomService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<ICustomerService, CustomerService>();
        services.AddScoped<IOrderService, OrderService>();

        return services;
    }
}
