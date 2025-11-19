using Investimentos.Domain.Interfaces;
using Investimentos.Infrastructure.Context;

namespace Investimentos.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private IClienteRepository? _clienteRepo;
    private IProdutoRepository? _produtoRepo;
    private IInvestimentoRepository? _investimentoRepo;
    private ISimulacaoRepository? _simulacaoRepo;

    public AppDbContext _context;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    public IClienteRepository ClienteRepository
    {
        get
        {
            return _clienteRepo = _clienteRepo ?? new ClienteRepository(_context);
        }
    }

    public IProdutoRepository ProdutoRepository
    {
        get
        {
            return _produtoRepo = _produtoRepo ?? new ProdutoRepository(_context);
        }
    }

    public ISimulacaoRepository SimulacaoRepository
    {
        get
        {
            return _simulacaoRepo = _simulacaoRepo ?? new SimulacaoRepository(_context);
        }
    }

    public IInvestimentoRepository InvestimentoRepository
    {
        get
        {
            return _investimentoRepo = _investimentoRepo ?? new InvestimentoRepository(_context);
        }
    }
    public async Task CommitAsync()
    {
        await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
