using Investimentos.Domain.Entities;

namespace Investimentos.Domain.Interfaces;

public interface IProdutoRepository : IRepository<Produto>
{
    Task<IEnumerable<Produto>> ObterTodosAsync();
    Task<IEnumerable<Produto>> ObterPorPerfilAsync(string perfil);
    Task<Produto> ObterPorTipoAsync(string type);
    Task<IEnumerable<Produto>> ObterTop3PorPerfilAsync(string perfil);
}
