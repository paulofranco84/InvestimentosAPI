namespace Investimentos.Application.DTOs;

public class SimulacoesResponseDTO
{
    public int Id { get; set; }
    public int ClienteId { get; set; }
    public string Produto { get; set; }
    public double ValorInvestido { get; set; }
    public double ValorFinal { get; set; }
    public int PrazoMeses { get; set; }
    public DateTime DataSimulacao { get; set; }
}
