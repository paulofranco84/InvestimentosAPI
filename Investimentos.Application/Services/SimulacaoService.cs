using Investimentos.Application.DTOs;
using Investimentos.Domain.Entities;
using Investimentos.Domain.Interfaces;
using System.Globalization;

namespace Investimentos.Application.Services;

public class SimulacaoService
{
    private readonly IUnitOfWork _uof;
    public SimulacaoService(IUnitOfWork unitOfWork)
    {
        _uof = unitOfWork;
    }

    public async Task<SimulacaoResponseDTO> SimularAsync(SimulacaoRequestDTO request)
    {
        var produto = await _uof.ProdutoRepository.ObterPorTipoAsync(request.TipoProduto);
        if (produto == null) throw new ArgumentException("Produto não encontrado.");

        if (request.PrazoMeses < produto.PrazoMinimo || request.PrazoMeses > produto.PrazoMaximo)
            throw new ArgumentException("Prazo Inválido.");

        var valorFinal = request.Valor * Math.Pow((double)(1 + produto.Rentabilidade), request.PrazoMeses / 12.0);

        var simulacao = new Simulacao
        {
            ClienteId = request.ClienteId,
            Produto = produto,
            ValorInvestido = request.Valor,
            ValorFinal = Math.Round(valorFinal,2),
            PrazoMeses = request.PrazoMeses,
            DataSimulacao = DateTime.UtcNow
        };

        await _uof.SimulacaoRepository.AdicionarAsync(simulacao);

        return new SimulacaoResponseDTO
        {
            ProdutoValidado = new ProdutoDTO
            {
                Id = produto.Id,
                Nome = produto.Nome,
                Tipo = produto.Tipo,
                Rentabilidade = produto.Rentabilidade,
                Risco = produto.Risco,
            },
            ResultadoSimulacao = new ResultadoSimulacao
            {
                ValorFinal = Math.Round(valorFinal,2).ToString("F2", CultureInfo.InvariantCulture),
                RentabilidadeEfetiva = (double)produto.Rentabilidade,
                PrazoMeses = request.PrazoMeses
            },
            DataSimulacao = simulacao.DataSimulacao
        };
    }
}
