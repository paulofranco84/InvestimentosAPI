using Investimentos.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Investimentos.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class PerfilRiscoController : ControllerBase
{
    private readonly IUnitOfWork _uof;
    private readonly IPerfilRiscoService _perfilService;

    public PerfilRiscoController(IUnitOfWork uof, IPerfilRiscoService perfilService)
    {
        _uof = uof;
        _perfilService = perfilService;
    }

    [HttpGet("{clienteId}")]
    public async Task<IActionResult> ObterPerfil(int clienteId)
    {
        var clienteSelecionado = _uof.ClienteRepository.GetAsync(c => c.Id == clienteId).Result;

        if (clienteSelecionado is null)
            return BadRequest("Cliente não encontrado");

        var perfil = await _perfilService.CalcularPerfilAsync(clienteId);
        return Ok(perfil);
    }
}
