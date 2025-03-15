using ProvaPub.Services.Interfaces;

namespace ProvaPub.Services;

public class NumberGeneratorService : INumberGeneratorService
{
    public NumberGeneratorService() { }

    public int Generate()
    {
        return Random.Shared.Next(1, int.MaxValue);
    }
}
