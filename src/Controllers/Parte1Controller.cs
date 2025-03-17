using Microsoft.AspNetCore.Mvc;
using ProvaPub.Services.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace ProvaPub.Controllers;

/// <summary>
/// Ao rodar o código abaixo o serviço deveria sempre retornar um número diferente, mas ele fica retornando sempre o mesmo número.
/// 1 - Faça as alterações para que o retorno seja sempre diferente
/// 2 - Tome cuidado 
/// </summary>
[ApiController]
[Route("[controller]")]
[ExcludeFromCodeCoverage]
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
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(int))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Index()
    {
        var number = await _randomService.GetRandom();

        return Ok(number);
    }
}
