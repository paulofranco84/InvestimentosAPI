using Investimentos.API.Middlewares;
using Investimentos.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Investimentos.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class TelemetriaController : ControllerBase
{
    private readonly TelemetryService _telemetryService;

    public TelemetriaController(TelemetryService telemetryService)
    {
        _telemetryService = telemetryService;
    }

    [HttpGet]
    public async Task<IActionResult> GetSummary([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
    {
        if (startDate.HasValue && endDate.HasValue && startDate > endDate)
        {
            return BadRequest(new { erro = "A data inicial não pode ser maior que a data final." });
        }

        var summary = await _telemetryService.GetSummaryAsync(startDate, endDate);
        return Ok(summary);
    }
}