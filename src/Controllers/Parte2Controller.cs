using Microsoft.AspNetCore.Mvc;
using ProvaPub.Models;
using ProvaPub.Services.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace ProvaPub.Controllers;

/// <summary>
/// Precisamos fazer algumas alterações:
/// 1 - Não importa qual page é informada, sempre são retornados os mesmos resultados. Faça a correção.
/// 2 - Altere os códigos abaixo para evitar o uso de "new", como em "new ProductService()". Utilize a Injeção de Dependência para resolver esse problema
/// 3 - Dê uma olhada nos arquivos /Models/CustomerList e /Models/ProductList. Veja que há uma estrutura que se repete. 
/// Como você faria pra criar uma estrutura melhor, com menos repetição de código? E quanto ao CustomerService/ProductService. Você acha que seria possível evitar a repetição de código?
/// 
/// </summary>
[ApiController]
[Route("[controller]")]
[ExcludeFromCodeCoverage]
public class Parte2Controller : ControllerBase
{
    private readonly IProductService _productService;
    private readonly ICustomerService _customerService;

    public Parte2Controller(IProductService productService, ICustomerService customerService)
    {
        _productService = productService;
        _customerService = customerService;
    }

    /// <summary>
    /// Obtém uma lista paginada de produtos.
    /// </summary>
    /// <param name="page">Número da página (começando em 1).</param>
    /// <returns>Retorna uma lista de produtos paginada.</returns>
    /// <response code="200">Lista de produtos retornada com sucesso.</response>
    /// <response code="400">Número da página inválido.</response>
    /// <response code="500">Erro interno ao recuperar os produtos.</response>
    [HttpGet("products")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PaginatedList<Product>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ListProducts(int page)
    {
        if (page < 1) return BadRequest("O número da página deve ser maior ou igual a 1.");

        var products = await _productService.ListProductsAsync(page);

        return Ok(products);
    }

    /// <summary>
    /// Obtém uma lista paginada de clientes.
    /// </summary>
    /// <param name="page">Número da página (começando em 1).</param>
    /// <returns>Retorna uma lista de clientes paginada.</returns>
    /// <response code="200">Lista de clientes retornada com sucesso.</response>
    /// <response code="400">Número da página inválido.</response>
    /// <response code="500">Erro interno ao recuperar os clientes.</response>
    [HttpGet("customers")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PaginatedList<Customer>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ListCustomers(int page)
    {
        if (page < 1) return BadRequest("O número da página deve ser maior ou igual a 1.");

        var customers = await _customerService.ListCustomersAsync(page);

        return Ok(customers);
    }
}
