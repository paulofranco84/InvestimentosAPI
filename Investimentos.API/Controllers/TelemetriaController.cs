using Investimentos.API.Middlewares;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Investimentos.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class TelemetriaController : ControllerBase
{
    [HttpGet]
    public IActionResult GetSummary([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
    {
        if (startDate.HasValue && endDate.HasValue && startDate > endDate)
        {
            return BadRequest(new { erro = "A data inicial não pode ser maior que a data final." });
        }

        var summary = MetricsAggregator.GetSummary(startDate, endDate);
        return Ok(summary);
    }
}