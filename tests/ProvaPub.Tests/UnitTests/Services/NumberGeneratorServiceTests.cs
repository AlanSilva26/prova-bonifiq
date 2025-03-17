using ProvaPub.Services;
using ProvaPub.Services.Interfaces;

namespace ProvaPub.Tests.UnitTests.Services;

public class NumberGeneratorServiceTests
{
    private readonly INumberGeneratorService _numberGeneratorService;

    public NumberGeneratorServiceTests() => _numberGeneratorService = new NumberGeneratorService();

    [Fact(DisplayName = "Generate() deve retornar um número dentro do intervalo válido")]
    public void Generate_ShouldReturnNumberWithinValidRange()
    {
        int result = _numberGeneratorService.Generate();

        Assert.InRange(result, 1, int.MaxValue);
    }

    [Fact(DisplayName = "Generate() deve gerar números diferentes em chamadas consecutivas")]
    public void Generate_ShouldReturnDifferentValues()
    {
        int result1 = _numberGeneratorService.Generate();
        int result2 = _numberGeneratorService.Generate();

        Assert.NotEqual(result1, result2);
    }

    [Fact(DisplayName = "Generate() nunca deve retornar zero")]
    public void Generate_ShouldNotReturnZero()
    {
        int result = _numberGeneratorService.Generate();

        Assert.NotEqual(0, result);
    }
}
