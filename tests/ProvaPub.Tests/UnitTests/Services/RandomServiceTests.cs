using Microsoft.Extensions.Logging;
using Moq;
using ProvaPub.Models;
using ProvaPub.Repository;
using ProvaPub.Repository.Interfaces;
using ProvaPub.Services;
using ProvaPub.Services.Interfaces;
using ProvaPub.Tests.Helpers;

namespace ProvaPub.Tests.UnitTests.Services;

public class RandomServiceTests
{
    private readonly Mock<IRandomNumberRepository> _randomNumberRepositoryMock;
    private readonly Mock<INumberGeneratorService> _numberGeneratorServiceMock;
    private readonly Mock<ILogger<RandomService>> _loggerMock;
    private readonly RandomService _randomService;

    public RandomServiceTests()
    {
        _randomNumberRepositoryMock = new Mock<IRandomNumberRepository>();
        _numberGeneratorServiceMock = new Mock<INumberGeneratorService>();
        _loggerMock = new Mock<ILogger<RandomService>>();

        _randomService = new RandomService(
            _randomNumberRepositoryMock.Object,
            _numberGeneratorServiceMock.Object,
            _loggerMock.Object
        );
    }

    [Fact(DisplayName = "GetRandom() deve retornar um número único na primeira tentativa")]
    public async Task GetRandom_ShouldReturnUniqueNumberOnFirstTry()
    {
        int uniqueNumber = 42;

        _numberGeneratorServiceMock.Setup(numberGeneratorService => numberGeneratorService.Generate()).Returns(uniqueNumber);
        _randomNumberRepositoryMock.Setup(randomNumberRepository => randomNumberRepository.ExistsAsync(uniqueNumber)).ReturnsAsync(false);

        int result = await _randomService.GetRandom();

        Assert.Equal(uniqueNumber, result);

        _randomNumberRepositoryMock.Verify(
            expression: randomNumberRepository => randomNumberRepository.SaveAsync(It.Is<RandomNumber>(randomNumber => randomNumber.Number == uniqueNumber)),
            times: Times.Once
        );
    }

    [Fact(DisplayName = "GetRandom() deve tentar novamente se gerar número duplicado")]
    public async Task GetRandom_ShouldRetryIfNumberIsDuplicate()
    {
        int duplicateNumber = 50;
        int uniqueNumber = 75;

        _numberGeneratorServiceMock
            .SetupSequence(n => n.Generate())
            .Returns(duplicateNumber)
            .Returns(uniqueNumber);

        _randomNumberRepositoryMock.Setup(randomNumberRepository => randomNumberRepository.ExistsAsync(duplicateNumber)).ReturnsAsync(true);
        _randomNumberRepositoryMock.Setup(randomNumberRepository => randomNumberRepository.ExistsAsync(uniqueNumber)).ReturnsAsync(false);

        int result = await _randomService.GetRandom();

        Assert.Equal(uniqueNumber, result);

        _randomNumberRepositoryMock.Verify(
            expression: randomNumberRepository => randomNumberRepository.ExistsAsync(duplicateNumber),
            times: Times.Once
        );

        _randomNumberRepositoryMock.Verify(
            expression: randomNumberRepository => randomNumberRepository.ExistsAsync(uniqueNumber),
            times: Times.Once
        );

        _randomNumberRepositoryMock.Verify(
            expression: randomNumberRepository => randomNumberRepository.SaveAsync(It.Is<RandomNumber>(n => n.Number == uniqueNumber)),
            times: Times.Once
        );
    }

    [Fact(DisplayName = "GetRandom() deve lançar exceção após 10 tentativas de gerar número único")]
    public async Task GetRandom_ShouldThrowExceptionAfterTenAttempts()
    {
        int duplicateNumber = 99;

        _numberGeneratorServiceMock.Setup(n => n.Generate()).Returns(duplicateNumber);
        _randomNumberRepositoryMock.Setup(r => r.ExistsAsync(duplicateNumber)).ReturnsAsync(true);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _randomService.GetRandom());
        Assert.Equal("Falha ao gerar um número único após múltiplas tentativas.", exception.Message);

        _randomNumberRepositoryMock.Verify(
            expression: r => r.ExistsAsync(duplicateNumber),
            times: Times.Exactly(10)
        );
    }

    [Fact(DisplayName = "GetRandom() deve registrar log quando gerar número duplicado")]
    public async Task GetRandom_ShouldLogWarningWhenNumberIsDuplicate()
    {
        int duplicateNumber = 88;
        int uniqueNumber = 77;

        _numberGeneratorServiceMock
            .SetupSequence(n => n.Generate())
            .Returns(duplicateNumber)
            .Returns(uniqueNumber);

        _randomNumberRepositoryMock.Setup(randomNumberRepository => randomNumberRepository.ExistsAsync(duplicateNumber)).ReturnsAsync(true);
        _randomNumberRepositoryMock.Setup(randomNumberRepository => randomNumberRepository.ExistsAsync(uniqueNumber)).ReturnsAsync(false);

        int result = await _randomService.GetRandom();

        Assert.Equal(uniqueNumber, result);

        _loggerMock.VerifyLogging(
            expectedMessage: $"Tentativa 1: Número duplicado ({duplicateNumber}). Gerando um novo...",
            expectedLogLevel: LogLevel.Warning
        );

        _randomNumberRepositoryMock.Verify(
            expression: randomNumberRepository => randomNumberRepository.ExistsAsync(It.IsAny<int>()),
            times: Times.Exactly(2)
        );

        _randomNumberRepositoryMock.Verify(
            expression: randomNumberRepository => randomNumberRepository.SaveAsync(It.IsAny<RandomNumber>()),
            times: Times.Once
        );
    }
}
