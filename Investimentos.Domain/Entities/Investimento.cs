namespace Investimentos.Domain.Entities;

public class Investimento
{
    public int Id { get; set; }
    public int ClienteId { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public double Valor { get; set; }
    public int PrazoMeses { get; set; }
    public decimal Rentabilidade { get; set; }
    public DateTime Data { get; set; }
}
