using Investimentos.Domain.Entities;
using Investimentos.Domain.Interfaces;
using Investimentos.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Investimentos.Infrastructure.Repositories;

public class ProdutoRepository : Repository<Produto>, IProdutoRepository
{
    public ProdutoRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Produto>> ObterTodosAsync() =>
        await _context.Produtos.ToListAsync();

    public async Task<IEnumerable<Produto>> ObterPorPerfilAsync(string perfil)
    {
        var perfilNormalizado = perfil.ToLowerInvariant();

        return perfilNormalizado switch
        {
            "conservador" => await _context.Produtos.Where(p => p.Risco == "Baixo").ToListAsync(),
            "moderado" => await _context.Produtos.Where(p => p.Risco != "Alto").ToListAsync(),
            "agressivo" => await _context.Produtos.ToListAsync(),
            _ => Enumerable.Empty<Produto>()
        };
    }

    public async Task<Produto?> ObterPorTipoAsync(string tipo)
    {
        return await _context.Produtos
            .FirstOrDefaultAsync(p => p.Tipo.ToLower() == tipo.ToLower());
    }
}
