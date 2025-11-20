using Investimentos.Domain.Entities;
using Investimentos.Domain.Interfaces;

namespace Investimentos.Application.Services;

public class PerfilRiscoService : IPerfilRiscoService
{
    private readonly IUnitOfWork _uof;
    public PerfilRiscoService(IUnitOfWork unitOfWork)
    {
        _uof = unitOfWork;
    }

    public async Task<PerfilRisco> CalcularPerfilAsync(int clienteId)
    {
        var investimentos = await _uof.InvestimentoRepository.ObterPorClienteAsync(clienteId);
        if (!investimentos.Any())
        {
            return new PerfilRisco
            {
                ClienteId = clienteId,
                Perfil = "Conservador",
                Pontuacao = 0,
                Descricao = "Sem investimentos registrados."
            };
        }

        // 1. Pontuação por risco
        double riscoTotal = 0;
        double valorTotal = investimentos.Sum(i => i.Valor);

        foreach (var i in investimentos)
        {
            double pesoRisco = i.Produto.Risco switch
            {
                "Baixo" => 0.2,
                "Médio" => 0.6,
                "Alto" => 1.0,
                _ => 0.5
            };

            riscoTotal += i.Valor * pesoRisco;
        }

        double riscoScore = (riscoTotal / valorTotal) * 100;

        // 2. Pontuação por frequência (máximo 100)
        int movimentacoes = investimentos.Count();
        double freqScore = Math.Min(movimentacoes * 10, 100);

        // 3. Pontuação por liquidez
        int liquidos = investimentos.Count(i => i.Tipo.Contains("Tesouro") || i.Tipo.Contains("CDB"));
        double liquidezScore = (liquidos / (double)movimentacoes) * 100;

        // 4. Cálculo final com pesos
        double pontuacaoFinal = (riscoScore * 0.5) + (freqScore * 0.3) + ((100 - liquidezScore) * 0.2);
        pontuacaoFinal = Math.Round(pontuacaoFinal, 2);

        // 5. Classificação
        string perfil = pontuacaoFinal switch
        {
            <= 40 => "Conservador",
            <= 70 => "Moderado",
            _ => "Agressivo"
        };

        string descricao = perfil switch
        {
            "Conservador" => "Perfil focado em segurança e liquidez.",
            "Moderado" => "Perfil equilibrado entre segurança e rentabilidade.",
            "Agressivo" => "Perfil voltado para alta rentabilidade e maior risco.",
            _ => "Indefinido"
        };

        return new PerfilRisco
        {
            ClienteId = clienteId,
            Perfil = perfil,
            Pontuacao = (int)pontuacaoFinal,
            Descricao = descricao
        };
    }
}