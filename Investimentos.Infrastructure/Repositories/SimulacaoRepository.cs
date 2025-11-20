using Investimentos.Domain.Entities;
using Investimentos.Domain.Interfaces;
using Investimentos.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Investimentos.Infrastructure.Repositories;

public class SimulacaoRepository : ISimulacaoRepository
{
    private readonly AppDbContext _context;

    public SimulacaoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AdicionarAsync(Simulacao simulacao)
    {
        _context.Simulacoes.Add(simulacao);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Simulacao>> ObterTodasAsync() =>
        await _context.Simulacoes.Include(s => s.Produto).ToListAsync();

    public async Task<IEnumerable<object>> ObterSimulacoesPorProdutoDiaAsync()
    {
        return await _context.Simulacoes
            .GroupBy(s => new { s.Produto.Nome, Data = s.DataSimulacao.Date })
            .Select(g => new
            {
                Produto = g.Key.Nome,
                Data = g.Key.Data.ToString("yyyy-MM-dd"),
                QuantidadeSimulacoes = g.Count(),
                MediaValorFinal = g.Average(s => s.ValorFinal).ToString("F2",CultureInfo.InvariantCulture)
            })
            .ToListAsync();
    }
}
