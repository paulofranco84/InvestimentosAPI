using Investimentos.Domain.Entities;
using Investimentos.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Investimentos.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ClientesController : ControllerBase
{
    private readonly IUnitOfWork _uof;
    public ClientesController(IUnitOfWork unitOfWork)
    {
        _uof = unitOfWork;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Cliente>>> GetAsync()
    {
        var clientes = await _uof.ClienteRepository.GetAllAsync();

        if (clientes is null)
        {
            return NotFound("Clientes não encontrados");
        }

        return Ok(clientes);
    }

    [HttpGet("{id:int}", Name = "ObterCliente")]
    public async Task<ActionResult<Cliente>> GetAsync(int id)
    {
        var cliente = await _uof.ClienteRepository.GetAsync(c => c.Id == id);

        if (cliente is null)
        {
            return NotFound("Cliente não encontrado");
        }

        return Ok(cliente);
    }

    [HttpPost]
    public async Task<ActionResult<Cliente>> PostAsync(Cliente _cliente)
    {
        if (_cliente is null)
            return BadRequest();

        var novoCliente = _uof.ClienteRepository.Create(_cliente);
        await _uof.CommitAsync();

        return new CreatedAtRouteResult("ObterCliente", new { id = novoCliente.Id }, novoCliente);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<Cliente>> PutAsync(int id, Cliente cliente)
    {
        if (id != cliente.Id)
            return BadRequest();

        var clienteAtualizado = _uof.ClienteRepository.Update(cliente);
        await _uof.CommitAsync();

        return Ok(clienteAtualizado);
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<Cliente>> DeleteAsync(int id)
    {
        var cliente = await _uof.ClienteRepository.GetAsync(c => c.Id == id);
        if (cliente is null)
            return NotFound("Cliente não encontrado...");

        var clienteDeletado = _uof.ClienteRepository.Delete(cliente);
        await _uof.CommitAsync();

        return Ok(clienteDeletado);
    }
}
