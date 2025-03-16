using Microsoft.EntityFrameworkCore;
using ProvaPub.Infra;
using ProvaPub.Repository;
using ProvaPub.Repository.Interfaces;
using ProvaPub.Services;
using ProvaPub.Services.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace ProvaPub.IoC;

[ExcludeFromCodeCoverage]
public static class ServiceCollection
{
    public static void AddServiceDefaults(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDatabase(configuration);
        services.AddRepositories();
        services.AddServices();
    }

    private static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("ctx");

        services.AddDbContext<TestDbContext>((services, builder) => builder.UseSqlServer(connectionString), ServiceLifetime.Singleton);

        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IRandomNumberRepository, RandomNumberRepository>();

        return services;
    }

    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<INumberGeneratorService, NumberGeneratorService>();
        services.AddScoped<IRandomService, RandomService>();

        return services;
    }
}
