using Microsoft.Extensions.Logging;
using Moq;
using ProvaPub.Models;
using ProvaPub.Repository.Interfaces;
using ProvaPub.Services;
using ProvaPub.Services.Interfaces;
using ProvaPub.Tests.Helpers;

namespace ProvaPub.Tests.UnitTests.Services;

public class RandomServiceTests
{
    private readonly Mock<IRandomNumberRepository> _repositoryMock;
    private readonly Mock<INumberGeneratorService> _numberGeneratorMock;
    private readonly Mock<ILogger<RandomService>> _loggerMock;
    private readonly RandomService _randomService;

    public RandomServiceTests()
    {
        _repositoryMock = new Mock<IRandomNumberRepository>();
        _numberGeneratorMock = new Mock<INumberGeneratorService>();
        _loggerMock = new Mock<ILogger<RandomService>>();

        _randomService = new RandomService(
            _repositoryMock.Object,
            _numberGeneratorMock.Object,
            _loggerMock.Object
        );
    }

    [Fact(DisplayName = "GetRandom() deve retornar um número único na primeira tentativa")]
    public async Task GetRandom_ShouldReturnUniqueNumberOnFirstTry()
    {
        int uniqueNumber = 42;
        _numberGeneratorMock.Setup(n => n.Generate()).Returns(uniqueNumber);
        _repositoryMock.Setup(r => r.ExistsAsync(uniqueNumber)).ReturnsAsync(false);

        int result = await _randomService.GetRandom();

        Assert.Equal(uniqueNumber, result);
        _repositoryMock.Verify(r => r.SaveAsync(It.Is<RandomNumber>(n => n.Number == uniqueNumber)), Times.Once);
    }

    [Fact(DisplayName = "GetRandom() deve tentar novamente se gerar número duplicado")]
    public async Task GetRandom_ShouldRetryIfNumberIsDuplicate()
    {
        int duplicateNumber = 50;
        int uniqueNumber = 75;

        _numberGeneratorMock
            .SetupSequence(n => n.Generate())
            .Returns(duplicateNumber)
            .Returns(uniqueNumber);

        _repositoryMock.Setup(r => r.ExistsAsync(duplicateNumber)).ReturnsAsync(true);
        _repositoryMock.Setup(r => r.ExistsAsync(uniqueNumber)).ReturnsAsync(false);

        int result = await _randomService.GetRandom();

        Assert.Equal(uniqueNumber, result);
        _repositoryMock.Verify(r => r.ExistsAsync(duplicateNumber), Times.Once);
        _repositoryMock.Verify(r => r.ExistsAsync(uniqueNumber), Times.Once);
        _repositoryMock.Verify(r => r.SaveAsync(It.Is<RandomNumber>(n => n.Number == uniqueNumber)), Times.Once);
    }

    [Fact(DisplayName = "GetRandom() deve lançar exceção após 10 tentativas de gerar número único")]
    public async Task GetRandom_ShouldThrowExceptionAfterTenAttempts()
    {
        int duplicateNumber = 99;

        _numberGeneratorMock.Setup(n => n.Generate()).Returns(duplicateNumber);
        _repositoryMock.Setup(r => r.ExistsAsync(duplicateNumber)).ReturnsAsync(true);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _randomService.GetRandom());
        Assert.Equal("Falha ao gerar um número único após múltiplas tentativas.", exception.Message);
        _repositoryMock.Verify(r => r.ExistsAsync(duplicateNumber), Times.Exactly(10));
    }

    [Fact(DisplayName = "GetRandom() deve registrar log quando gerar número duplicado")]
    public async Task GetRandom_ShouldLogWarningWhenNumberIsDuplicate()
    {
        int duplicateNumber = 88;
        int uniqueNumber = 77;

        _numberGeneratorMock
            .SetupSequence(n => n.Generate())
            .Returns(duplicateNumber)
            .Returns(uniqueNumber);

        _repositoryMock.Setup(r => r.ExistsAsync(duplicateNumber)).ReturnsAsync(true);
        _repositoryMock.Setup(r => r.ExistsAsync(uniqueNumber)).ReturnsAsync(false);

        int result = await _randomService.GetRandom();

        Assert.Equal(uniqueNumber, result);
        _loggerMock.VerifyLogging(expectedMessage: $"Tentativa 1: Número duplicado ({duplicateNumber}). Gerando um novo...", expectedLogLevel: LogLevel.Warning);
    }
}
