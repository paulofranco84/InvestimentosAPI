using Investimentos.Application.DTOs;
using Investimentos.Application.Services;
using Investimentos.Domain.Entities;
using Investimentos.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Investimentos.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class SimulacoesController : ControllerBase
{
    private readonly SimulacaoService _simulacaoService;
    private readonly IUnitOfWork _uof;

    public SimulacoesController(SimulacaoService simulacaoService, IUnitOfWork uof)
    {
        _simulacaoService = simulacaoService;
        _uof = uof;
    }

    [HttpPost("simular-investimento")]
    public async Task<IActionResult> Simular(SimulacaoRequestDTO request)
    {
        var clienteSelecionado = _uof.ClienteRepository.GetAsync(c => c.Id == request.ClienteId).Result;

        if (clienteSelecionado is null)
            return BadRequest("Cliente não encontrado");

        var produtoSelecionado = _uof.ProdutoRepository.GetAsync(p => p.Tipo.ToLower() == request.TipoProduto.ToLower() && 
            p.PrazoMinimo <= request.PrazoMeses && p.PrazoMaximo >= request.PrazoMeses).Result;

        if (produtoSelecionado is null)
            return BadRequest("Produto com tipo e prazo informados não encontrados");

        if (request.PrazoMeses < produtoSelecionado.PrazoMinimo || request.PrazoMeses > produtoSelecionado.PrazoMaximo)
            return BadRequest("Prazo inválido");

        var resultado = await _simulacaoService.SimularAsync(request);
        return Ok(resultado);
    }

    [HttpGet("simulacoes")]
    public async Task<IActionResult> ObterSimulacoes()
    {

        var simulacoes = await _uof.SimulacaoRepository.ObterTodasAsync();

        var response = simulacoes.Select(s => new SimulacoesResponseDTO
        {
            Id = s.Id,
            ClienteId = s.ClienteId,
            Produto = s.Produto.Nome,
            ValorInvestido = s.ValorInvestido,
            ValorFinal = s.ValorFinal,
            PrazoMeses = s.PrazoMeses,
            DataSimulacao = s.DataSimulacao
        });

        return Ok(response);
    }

    [HttpGet("simulacoes/por-produto-dia")]
    public async Task<IActionResult> SimulacoesPorProdutoEDia()
    {
        var dados = await _uof.SimulacaoRepository.ObterSimulacoesPorProdutoDiaAsync();
        return Ok(dados);
    }
}
