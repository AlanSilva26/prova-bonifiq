using Microsoft.AspNetCore.Mvc;
using ProvaPub.Services.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace ProvaPub.Controllers;


/// <summary>
/// O Código abaixo faz uma chmada para a regra de negócio que valida se um consumidor pode fazer uma compra.
/// Crie o teste unitário para esse Service. Se necessário, faça as alterações no código para que seja possível realizar os testes.
/// Tente criar a maior cobertura possível nos testes.
/// 
/// Utilize o framework de testes que desejar. 
/// Crie o teste na pasta "Tests" da solution
/// </summary>
[ApiController]
[Route("[controller]")]
[ExcludeFromCodeCoverage]
public class Parte4Controller : ControllerBase
{
    private readonly ICustomerService _customerService;

    public Parte4Controller(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    /// <summary>
    /// Verifica se o cliente pode realizar uma compra com base nas regras de negócio.
    /// </summary>
    /// <param name="customerId">ID do cliente.</param>
    /// <param name="purchaseValue">Valor da compra.</param>
    /// <returns>Retorna <c>true</c> se o cliente puder realizar a compra, caso contrário, <c>false</c>.</returns>
    /// <response code="200">Indica se o cliente pode ou não fazer a compra.</response>
    /// <response code="400">Erro de validação: valor da compra inválido ou regras de negócio não atendidas.</response>
    /// <response code="404">Cliente não encontrado na base de dados.</response>
    /// <response code="500">Erro interno no processamento da requisição.</response>
    [HttpGet("CanPurchase")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CanPurchase(int customerId, decimal purchaseValue)
    {
        try
        {
            bool canPurchase = await _customerService.CanPurchase(customerId, purchaseValue);

            return Ok(canPurchase);
        }
        catch (ArgumentOutOfRangeException exception)
        {
            return BadRequest(new { message = exception.Message });
        }
        catch (InvalidOperationException exception)
        {
            return NotFound(new { message = exception.Message });
        }
        catch (Exception exception)
        {
            return StatusCode(500, new { message = "Erro interno no servidor. Tente novamente mais tarde.", error = exception.Message });
        }
    }
}
