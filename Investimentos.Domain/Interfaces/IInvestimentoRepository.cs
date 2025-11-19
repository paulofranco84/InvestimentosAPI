using Investimentos.Domain.Entities;

namespace Investimentos.Domain.Interfaces;

public interface IInvestimentoRepository
{
    Task Adicionar(Investimento investimento);
    Task<IEnumerable<Investimento>> ObterPorClienteAsync(int clienteId);
    Task<Investimento>GetAsync(int investimentoId);
}
