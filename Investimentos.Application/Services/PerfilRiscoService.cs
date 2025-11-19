using Investimentos.Domain.Entities;
using Investimentos.Domain.Interfaces;

namespace Investimentos.Application.Services;

public class PerfilRiscoService : IPerfilRiscoService
{
    private readonly IUnitOfWork _uof;
    public PerfilRiscoService(IUnitOfWork unitOfWork)
    {
        _uof = unitOfWork;
    }

    public async Task<PerfilRisco> CalcularPerfilAsync(int clienteId)
    {
        var investimentos = await _uof.InvestimentoRepository.ObterPorClienteAsync(clienteId);
        var total = investimentos.Sum(i => i.Valor);
        var frequencia = investimentos.Count();
        var liquidez = investimentos.Count(i => i.Tipo.Contains("Tesouro") || i.Tipo.Contains("CDB"));

        var pontuacao = (int)(total / 1000) + frequencia * 5 + liquidez * 3;

        var perfil = pontuacao switch
        {
            <= 40 => "Conservador",
            <= 70 => "Moderado",
            _ => "Agressivo"
        };

        var descricao = perfil switch
        {
            "Conservador" => "Perfil focado em segurança e liquidez.",
            "Moderado" => "Perfil equilibrado entre segurança e rentabilidade.",
            "Agressivo" => "Perfil voltado para alta rentabilidade e maior risco.",
            _ => "Indefinido"
        };

        return new PerfilRisco
        {
            ClienteId = clienteId,
            Perfil = perfil,
            Pontuacao = pontuacao,
            Descricao = descricao
        };
    }
}
