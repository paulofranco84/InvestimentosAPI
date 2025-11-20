namespace Investimentos.Application.DTOs;

public class InvestimentoResponseDTO
{
    public int Id { get; set; }

    public string? tipo { get; set; }
    public double Valor { get; set; }
    public decimal Rentabilidade { get; set; }
    public string? Data { get; set; }
}