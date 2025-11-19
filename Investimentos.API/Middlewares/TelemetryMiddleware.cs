namespace Investimentos.API.Middlewares;

using System.Diagnostics.Metrics;

public class TelemetryMiddleware
{
    private readonly RequestDelegate _next;
    private static readonly Meter Meter = new("InvestimentosApi.Metrics", "1.0");
    private static readonly Counter<int> RequestCounter = Meter.CreateCounter<int>("api_requests_total");
    private static readonly Histogram<double> ResponseTimeHistogram = Meter.CreateHistogram<double>("api_response_time_ms");

    public TelemetryMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        var start = DateTime.UtcNow;
        await _next(context);
        var elapsed = (DateTime.UtcNow - start).TotalMilliseconds;

        var endpoint = context.Request.Path.ToString();
        RequestCounter.Add(1, new KeyValuePair<string, object?>("endpoint", endpoint));
        ResponseTimeHistogram.Record(elapsed, new KeyValuePair<string, object?>("endpoint", endpoint));

        MetricsAggregator.Add(endpoint, elapsed);
    }
}

public static class MetricsAggregator
{
    private static readonly Dictionary<string, List<(DateTime Timestamp, double Elapsed)>> Data = new();

    public static void Add(string endpoint, double elapsed)
    {
        lock (Data)
        {
            if (!Data.ContainsKey(endpoint))
                Data[endpoint] = new List<(DateTime, double)>();

            Data[endpoint].Add((DateTime.UtcNow, elapsed));
        }
    }

    public static object GetSummary(DateTime? startDate = null, DateTime? endDate = null)
    {
        lock (Data)
        {
            var inicio = startDate ?? DateTime.UtcNow.AddDays(-1); // default: último dia
            var fim = (endDate?.Date ?? DateTime.UtcNow.Date).AddDays(1).AddTicks(-1); // Isso define fim como 23:59:59.999 do dia informado


            var servicos = Data.Select(kv =>
            {
                var registros = kv.Value
                    .Where(r => r.Timestamp >= inicio && r.Timestamp <= fim)
                    .ToList();

                var count = registros.Count;
                var totalTime = registros.Sum(r => r.Elapsed);

                return new
                {
                    nome = kv.Key.Replace("api/", "").Trim('/'),
                    quantidadeChamadas = count,
                    mediaTempoRespostaMs = count > 0 ? Math.Round(totalTime / count, 2) : 0
                };
            }).ToList();

            return new
            {
                servicos,
                periodo = new
                {
                    inicio = inicio.ToString("yyyy-MM-dd"),
                    fim = fim.ToString("yyyy-MM-dd")
                }
            };
        }
    }
}