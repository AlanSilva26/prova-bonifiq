using Microsoft.AspNetCore.Mvc;
using ProvaPub.Services.Interfaces;

namespace ProvaPub.Controllers
{
    /// <summary>
    /// Ao rodar o código abaixo o serviço deveria sempre retornar um número diferente, mas ele fica retornando sempre o mesmo número.
    /// 1 - Faça as alterações para que o retorno seja sempre diferente
    /// 2 - Tome cuidado 
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class Parte1Controller : ControllerBase
    {
        private readonly IRandomService _randomService;

        public Parte1Controller(IRandomService randomService) => _randomService = randomService;

        /// <summary>
        /// Obtém um número aleatório único.
        /// </summary>
        /// <returns>Retorna um número aleatório único.</returns>
        /// <response code="200">Número gerado com sucesso.</response>
        /// <response code="500">Erro interno ao gerar o número.</response>
        [HttpGet]
        [ProducesResponseType(typeof(int), 200)]
        [ProducesResponseType(500)]
        public async Task<int> Index()
        {
            return await _randomService.GetRandom();
        }
    }
}
