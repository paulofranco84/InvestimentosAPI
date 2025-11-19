namespace Investimentos.Domain.Entities;

public class PerfilRisco
{
    public int ClienteId { get; set; }
    public string Perfil { get; set; } = string.Empty;
    public int Pontuacao { get; set; }
    public string Descricao { get; set; } = string.Empty;
}
