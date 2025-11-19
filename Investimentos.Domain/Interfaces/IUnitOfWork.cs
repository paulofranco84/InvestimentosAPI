namespace Investimentos.Domain.Interfaces;

public interface IUnitOfWork
{
    IClienteRepository ClienteRepository { get; }
    IProdutoRepository ProdutoRepository { get; }
    ISimulacaoRepository SimulacaoRepository { get; }
    IInvestimentoRepository InvestimentoRepository { get; }
    Task CommitAsync();
}
