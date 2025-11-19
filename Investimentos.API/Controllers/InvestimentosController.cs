using Investimentos.Application.DTOs;
using Investimentos.Domain.Entities;
using Investimentos.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Investimentos.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class InvestimentosController : ControllerBase
{
    private readonly IUnitOfWork _uof;
    public InvestimentosController(IUnitOfWork unitOfWork)
    {
        _uof = unitOfWork;
    }

    [HttpGet("ObterPorCliente/{clienteId}")]
    public async Task<IActionResult> Obter(int clienteId)
    {
        var cliente = await _uof.ClienteRepository.GetAsync(c => c.Id == clienteId);
        if (cliente is null) return NotFound("Cliente não cadastrado");

        var investimentos = await _uof.InvestimentoRepository.ObterPorClienteAsync(clienteId);
        return Ok(investimentos);
    }

    [HttpPost("Investir")]
    public async Task<IActionResult> Investir(InvestimentoRequestDTO _investimento)
    {
        var clienteSelecionado = _uof.ClienteRepository.GetAsync(c => c.Id == _investimento.ClienteId).Result;
        
        if (clienteSelecionado is null)
            return BadRequest("Cliente não encontrado");

        var produtoSelecionado = _uof.ProdutoRepository.GetAsync(p => p.Id == _investimento.ProdutoId).Result;

        if (produtoSelecionado is null)
            return BadRequest("Produto não encontrado");

        if (_investimento.PrazoMeses < produtoSelecionado.PrazoMinimo || _investimento.PrazoMeses > produtoSelecionado.PrazoMaximo)
            return BadRequest("Prazo inválido");

        var novoInvestimento = new Investimento
        {
            ClienteId = _investimento.ClienteId,
            Valor = _investimento.Valor,
            PrazoMeses = _investimento.PrazoMeses,
            Rentabilidade = produtoSelecionado.Rentabilidade,
            Tipo = produtoSelecionado.Tipo,
            Data = DateTime.UtcNow
        };
        
        var retorno = _uof.InvestimentoRepository.Adicionar(novoInvestimento);
        await _uof.CommitAsync();

        return Ok(novoInvestimento);
    }

    [HttpGet("{investimentoId:int}", Name = "ObterInvestimento")]
    public async Task<ActionResult<Investimento>> GetAsync(int investimentoId)
    {
        var investimento = await _uof.InvestimentoRepository.GetAsync(investimentoId);

        if (investimento is null)
        {
            return NotFound("Investimento não encontrado");
        }

        return Ok(investimento);
    }
}
