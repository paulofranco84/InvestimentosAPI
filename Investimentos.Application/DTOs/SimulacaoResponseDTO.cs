namespace Investimentos.Application.DTOs;

public class SimulacaoResponseDTO
{
    public ProdutoDTO ProdutoValidado { get; set; } = null!;
    public ResultadoSimulacao ResultadoSimulacao { get; set; } = null!;
    public DateTime DataSimulacao { get; set; }
}

public class ResultadoSimulacao
{
    public string? ValorFinal { get; set; }
    public double RentabilidadeEfetiva { get; set; }
    public int PrazoMeses { get; set; }
}
