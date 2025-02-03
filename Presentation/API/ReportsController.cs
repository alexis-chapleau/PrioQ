using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PrioQ.Application.Interfaces; // Contains AnalyticsReportUseCase

namespace PrioQ.Presentation.API
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportsController : ControllerBase
    {
        private readonly IGetAnalyticsReportUseCase _reportUseCase;

        public ReportsController(IGetAnalyticsReportUseCase reportUseCase)
        {
            _reportUseCase = reportUseCase;
        }

        /// <summary>
        /// GET api/reports?from=2025-01-01T00:00:00Z&to=2025-01-08T00:00:00Z
        /// </summary>
        /// <param name="from">The start date/time of the report period (UTC). Defaults to 7 days ago if not specified.</param>
        /// <param name="to">The end date/time of the report period (UTC). Defaults to now if not specified.</param>
        /// <returns>A JSON report containing analytics data.</returns>
        [HttpGet]
        public IActionResult GetReport([FromQuery] DateTime? from, [FromQuery] DateTime? to)
        {
            // Use default values if query parameters are missing.
            var reportFrom = from ?? DateTime.UtcNow.AddDays(-7);
            var reportTo = to ?? DateTime.UtcNow;

            // Generate the report using the use case.
            var report = _reportUseCase.GenerateReport(reportFrom, reportTo);

            // Return the report as JSON.
            return Ok(report);
        }
    }
}
