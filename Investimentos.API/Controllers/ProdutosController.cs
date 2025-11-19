using Investimentos.Domain.Entities;
using Investimentos.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Investimentos.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class ProdutosController : ControllerBase
{
    private readonly IUnitOfWork _uof;
    public ProdutosController(IUnitOfWork unitOfWork)
    {
        _uof = unitOfWork;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Produto>>> GetAsync()
    {
        var produtos = await _uof.ProdutoRepository.GetAllAsync();

        if (produtos is null)
        {
            return NotFound("Produtos não encontrados");
        }

        return Ok(produtos);
    }

    [HttpGet("{id:int}", Name = "ObterProduto")]
    public async Task<ActionResult<Produto>> GetAsync(int id)
    {
        var produto = await _uof.ProdutoRepository.GetAsync(p => p.Id == id);

        if (produto is null)
        {
            return NotFound("Produto não encontrado");
        }

        return Ok(produto);
    }

    [HttpPost]
    public async Task<ActionResult<Produto>> PostAsync(Produto _produto)
    {
        if (_produto is null)
            return BadRequest();

        var novoProduto = _uof.ProdutoRepository.Create(_produto);
        await _uof.CommitAsync();

        return new CreatedAtRouteResult("ObterProduto", new { id = novoProduto.Id }, novoProduto);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<Produto>> PutAsync(int id, Produto produto)
    {
        if (id != produto.Id)
            return BadRequest();

        var produtoAtualizado = _uof.ProdutoRepository.Update(produto);
        await _uof.CommitAsync();

        return Ok(produtoAtualizado);
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<Produto>> DeleteAsync(int id)
    {
        var produto = await _uof.ProdutoRepository.GetAsync(p => p.Id == id);
        if (produto is null)
            return NotFound("Produto não encontrado...");

        var produtoDeletado = _uof.ProdutoRepository.Delete(produto);
        await _uof.CommitAsync();

        return Ok(produtoDeletado);
    }

    [HttpGet("produtos-recomendados/{perfil}")]
    public async Task<IActionResult> Recomendados(string perfil)
    {
        var produtos = await _uof.ProdutoRepository.ObterPorPerfilAsync(perfil);

        if (!produtos.Any()) return BadRequest("Perfil inválido");

        return Ok(produtos);
    }
}
