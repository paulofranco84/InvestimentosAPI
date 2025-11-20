using Investimentos.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Investimentos.Application.Services;

public class TelemetryService
{
    private readonly AppDbContext _context;

    public TelemetryService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<object> GetSummaryAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        var inicio = startDate ?? DateTime.UtcNow.AddDays(-1);
        var fim = (endDate?.Date ?? DateTime.UtcNow.Date).AddDays(1).AddTicks(-1);

        var registros = await _context.RegistrosTelemetria
            .Where(r => r.Timestamp >= inicio && r.Timestamp <= fim)
            .ToListAsync();

        var agrupado = registros
            .GroupBy(r => r.Endpoint)
            .Select(g => new
            {
                nome = g.Key,
                quantidadeChamadas = g.Count(),
                mediaTempoRespostaMs = Math.Round(g.Average(r => r.TempoRespostaMs), 2)
            })
            .ToList();

        return new
        {
            servicos = agrupado,
            periodo = new
            {
                inicio = inicio.ToString("yyyy-MM-dd"),
                fim = fim.ToString("yyyy-MM-dd")
            }
        };
    }
}