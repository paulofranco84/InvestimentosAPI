using Investimentos.Domain.Entities;

namespace Investimentos.Domain.Interfaces;

public interface ISimulacaoRepository
{
    Task AdicionarAsync(Simulacao simulacao);
    Task<IEnumerable<Simulacao>> ObterTodasAsync();
    Task<IEnumerable<object>> ObterSimulacoesPorProdutoDiaAsync();
}
