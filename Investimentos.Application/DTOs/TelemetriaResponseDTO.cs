namespace Investimentos.Application.DTOs;

public class TelemetriaResponseDTO
{
    public List<ServicoTelemetria> Servicos { get; set; } = new();
    public PeriodoTelemetria Periodo { get; set; } = new();
}

public class ServicoTelemetria
{
    public string Nome { get; set; } = string.Empty;
    public int QuantidadeChamadas { get; set; }
    public double MediaTempoRespostaMs { get; set; }
}

public class PeriodoTelemetria
{
    public DateTime Inicio { get; set; }
    public DateTime Fim { get; set; }
}
