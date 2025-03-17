using Microsoft.EntityFrameworkCore;
using ProvaPub.Infra;
using ProvaPub.Models;
using ProvaPub.Repository;

namespace ProvaPub.Tests.UnitTests.Repository
{
    public class RandomNumberRepositoryTests
    {
        private readonly TestDbContext _dbContext;
        private readonly RandomNumberRepository _randomNumberRepository;

        public RandomNumberRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _dbContext = new TestDbContext(options);
            _randomNumberRepository = new RandomNumberRepository(_dbContext);
        }

        [Fact(DisplayName = "Deve verificar se um número já existe no banco de dados")]
        public async Task ExistsAsync_ShouldReturnTrue_WhenNumberExists()
        {
            var number = new RandomNumber { Number = 42 };
            await _dbContext.Numbers.AddAsync(number);
            await _dbContext.SaveChangesAsync();

            var result = await _randomNumberRepository.ExistsAsync(42);

            Assert.True(result);
        }

        [Fact(DisplayName = "Deve verificar que um número não existe no banco de dados")]
        public async Task ExistsAsync_ShouldReturnFalse_WhenNumberDoesNotExist()
        {
            var result = await _randomNumberRepository.ExistsAsync(99);
            Assert.False(result);
        }

        [Fact(DisplayName = "Deve salvar um número aleatório no banco de dados")]
        public async Task SaveAsync_ShouldSaveRandomNumberSuccessfully()
        {
            var randomNumber = new RandomNumber { Number = 123 };

            await _randomNumberRepository.SaveAsync(randomNumber);
            var exists = await _dbContext.Numbers.AnyAsync(n => n.Number == 123);

            Assert.True(exists);
        }
    }
}
