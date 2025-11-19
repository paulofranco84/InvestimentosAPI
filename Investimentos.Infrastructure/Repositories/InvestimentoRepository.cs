using Investimentos.Domain.Entities;
using Investimentos.Domain.Interfaces;
using Investimentos.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Investimentos.Infrastructure.Repositories;

public class InvestimentoRepository : IInvestimentoRepository
{
    private readonly AppDbContext _context;

    public InvestimentoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Investimento>> ObterPorClienteAsync(int clienteId) =>
        await _context.Investimentos.Where(i => i.ClienteId == clienteId).ToListAsync();

    public async Task Adicionar(Investimento _investimento)
    {
        _context.Investimentos.Add(_investimento);
        await _context.SaveChangesAsync();
    }

    public async Task<Investimento> GetAsync(int investimentoId)
    {
        return await _context.Investimentos.FirstOrDefaultAsync(i => i.Id == investimentoId);
    }
}
