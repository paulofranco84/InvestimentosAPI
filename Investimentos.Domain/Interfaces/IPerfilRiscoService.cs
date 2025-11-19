using Investimentos.Domain.Entities;

namespace Investimentos.Domain.Interfaces;

public interface IPerfilRiscoService
{
    Task<PerfilRisco> CalcularPerfilAsync(int clienteId);
}
