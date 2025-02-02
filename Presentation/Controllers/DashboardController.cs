using System;
using Microsoft.AspNetCore.Mvc;
using PrioQ.Application.DTOs;
using PrioQ.Application.Interfaces;

namespace PrioQ.UI.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IGetAnalyticsReportUseCase _reportUseCase;

        public DashboardController(IGetAnalyticsReportUseCase reportUseCase)
        {
            _reportUseCase = reportUseCase;
        }

        // GET: /Dashboard/Index?from=2025-01-01T00:00:00Z&to=2025-01-08T00:00:00Z
        public IActionResult Index(DateTime? from, DateTime? to)
        {
            // Default to the past 7 days if not specified.
            DateTime reportFrom = from ?? DateTime.UtcNow.AddDays(-7);
            DateTime reportTo = to ?? DateTime.UtcNow;

            AnalyticsReportDto report = _reportUseCase.GenerateReport(reportFrom, reportTo);

            return View(report);
        }
    }
}
