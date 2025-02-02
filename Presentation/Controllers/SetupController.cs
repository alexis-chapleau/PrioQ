using Microsoft.AspNetCore.Mvc;
using PrioQ.Application.Interfaces;
using PrioQ.Domain.Entities;
using PrioQ.Infrastructure.Configuration; // or your correct namespace
using PrioQ.UI.Models;

namespace PrioQ.UI.Controllers
{
    public class SetupController : Controller
    {
        private readonly IConfigProvider _configProvider;
        private readonly IInitializeQueueUseCase _initializeQueueUseCase;

        public SetupController(IInitializeQueueUseCase initializeQueueUseCase, IConfigProvider configProvider)
        {
            _initializeQueueUseCase = initializeQueueUseCase;
            _configProvider = configProvider;
        }

        [HttpGet]
        public IActionResult Index()
        {
            // Read default config from config.json
            var defaultConfig = _configProvider.GetQueueConfig();

            // Map it to SetupViewModel
            var model = new SetupViewModel
            {
                UnboundedPriority = defaultConfig.UnboundedPriority,
                MaxPriority = defaultConfig.MaxPriority,
                Algorithm = defaultConfig.Algorithm, // if using an enum
                UseLogging = defaultConfig.UseLogging,
                UseLocking = defaultConfig.UseLocking,
                UseLazyDelete = defaultConfig.UseLazyDelete,
                UseAnalytics = defaultConfig.UseAnalytics
            };

            // Return the normal HTML form (Views/Setup/Index.cshtml)
            return View(model);
        }

        [HttpPost]
        public IActionResult Index(SetupViewModel model)
        {
            // Reconstruct a new QueueConfig
            var config = new QueueConfig
            {
                UnboundedPriority = model.UnboundedPriority,
                MaxPriority = model.MaxPriority,
                Algorithm = model.Algorithm,
                UseLogging = model.UseLogging,
                UseLocking = model.UseLocking,
                UseLazyDelete = model.UseLazyDelete,
                UseAnalytics = model.UseAnalytics
            };
            _initializeQueueUseCase.Execute(config);

            TempData["Message"] = "Queue created successfully with your normal HTML form!";
            // Redirect or return view as needed
            return RedirectToAction("Index", "Dashboard");
        }
    }
}
