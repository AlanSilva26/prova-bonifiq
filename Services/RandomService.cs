using ProvaPub.Models;
using ProvaPub.Repository.Interfaces;
using ProvaPub.Services.Interfaces;

namespace ProvaPub.Services;

public class RandomService : IRandomService
{
    private readonly IRandomNumberRepository _repository;
    private readonly INumberGeneratorService _numberGeneratorService;
    private readonly ILogger<RandomService> _logger;

    public RandomService(IRandomNumberRepository repository, INumberGeneratorService numberGeneratorService, ILogger<RandomService> logger)
    {
        _repository = repository;
        _numberGeneratorService = numberGeneratorService;
        _logger = logger;
    }

    public async Task<int> GetRandom()
    {
        for (int attempts = 0; attempts < 10; attempts++)
        {
            int number = _numberGeneratorService.Generate();

            if (!await _repository.ExistsAsync(number))
            {
                await _repository.SaveAsync(new RandomNumber { Number = number });

                return number;
            }

            _logger.LogWarning($"Tentativa {attempts + 1}: Número duplicado ({number}). Gerando um novo...");
        }

        throw new InvalidOperationException("Falha ao gerar um número único após múltiplas tentativas.");
    }
}
