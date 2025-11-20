namespace Investimentos.API.Middlewares;

using Investimentos.Infrastructure.Context;
using Investimentos.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
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
        var _context = context.RequestServices.GetRequiredService<AppDbContext>();
        var start = DateTime.UtcNow;
        await _next(context);
        var elapsed = (DateTime.UtcNow - start).TotalMilliseconds;

        var rawPattern = (context.GetEndpoint() as Microsoft.AspNetCore.Routing.RouteEndpoint)?
            .RoutePattern
            .RawText ?? context.Request.Path.ToString();

        var routePattern = string.Join('/',
            rawPattern.Split('/', StringSplitOptions.RemoveEmptyEntries)
                      .Where(segment => !segment.StartsWith("{"))
        );

        RequestCounter.Add(1, new KeyValuePair<string, object?>("endpoint", routePattern));
        ResponseTimeHistogram.Record(elapsed, new KeyValuePair<string, object?>("endpoint", routePattern));
        _context.RegistrosTelemetria.Add(new RegistroTelemetria
        {
            Endpoint = routePattern,
            Timestamp = DateTime.UtcNow,
            TempoRespostaMs = elapsed
        });

        await _context.SaveChangesAsync();
    }
}