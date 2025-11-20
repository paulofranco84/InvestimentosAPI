namespace Investimentos.Infrastructure.Models;

public class RegistroTelemetria
{
    public int Id { get; set; }
    public string Endpoint { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public double TempoRespostaMs { get; set; }
}
