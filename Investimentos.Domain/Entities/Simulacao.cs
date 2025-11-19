namespace Investimentos.Domain.Entities;

public class Simulacao
{
    public int Id { get; set; }
    public int ClienteId { get; set; }
    public Produto Produto { get; set; } = null!;
    public double ValorInvestido { get; set; }
    public double ValorFinal { get; set; }
    public int PrazoMeses { get; set; }
    public DateTime DataSimulacao { get; set; }
}
