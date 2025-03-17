using Microsoft.AspNetCore.Mvc;
using ProvaPub.Models;
using ProvaPub.Services.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace ProvaPub.Controllers;


/// <summary>
/// Esse teste simula um pagamento de uma compra.
/// O método PayOrder aceita diversas formas de pagamento. Dentro desse método é feita uma estrutura de diversos "if" para cada um deles.
/// Sabemos, no entanto, que esse formato não é adequado, em especial para futuras inclusões de formas de pagamento.
/// Como você reestruturaria o método PayOrder para que ele ficasse mais aderente com as boas práticas de arquitetura de sistemas?
/// 
/// Outra parte importante é em relação à data (OrderDate) do objeto Order. Ela deve ser salva no banco como UTC mas deve retornar para o cliente no fuso horário do Brasil. 
/// Demonstre como você faria isso.
/// </summary>
[ApiController]
[Route("[controller]")]
[ExcludeFromCodeCoverage]
public class Parte3Controller : ControllerBase
{
    private readonly IOrderService _orderService;

    public Parte3Controller(IOrderService orderService) => _orderService = orderService;

    /// <summary>
    /// Processa o pagamento de um pedido com o método de pagamento especificado.
    /// </summary>
    /// <param name="paymentMethod">Método de pagamento (Pix, CreditCard, PayPal).</param>
    /// <param name="value">Valor do pagamento.</param>
    /// <param name="customerId">ID do cliente que está realizando o pagamento.</param>
    /// <returns>Retorna os detalhes do pedido processado.</returns>
    /// <response code="200">Pedido processado com sucesso.</response>
    /// <response code="400">Requisição inválida (dados ausentes ou incorretos).</response>
    /// <response code="500">Erro interno ao processar o pedido.</response>
    [HttpPost("PlaceOrder")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Order))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Order>> PlaceOrder(string paymentMethod, decimal value, int customerId)
    {
        if (value <= 0) return BadRequest("O valor do pagamento deve ser maior que zero.");

        try
        {
            var order = await _orderService.PayOrderAsync(paymentMethod, value, customerId);

            return Ok(order);
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(exception.Message);
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Erro interno ao processar o pedido.");
        }
    }
}
