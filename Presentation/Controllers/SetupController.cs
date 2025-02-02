using Microsoft.AspNetCore.Mvc;
using PrioQ.Domain.Entities;
using PrioQ.Application.Interfaces; // If you have IConfigProvider in a shared or Application project; otherwise adjust the namespace.
using PrioQ.Infrastructure.Configuration;
using PrioQ.UI.Models;
using System;

namespace PrioQ.UI.Controllers
{
    public class SetupController : Controller
    {
        private readonly IInitializeQueueUseCase _initializeQueueUseCase;
        private readonly IConfigProvider _configProvider;

        public SetupController(IInitializeQueueUseCase initializeQueueUseCase, IConfigProvider configProvider)
        {
            _initializeQueueUseCase = initializeQueueUseCase;
            _configProvider = configProvider;
        }

        [HttpGet]
        public IActionResult Index()
        {
            // Get the default configuration (from config.json)
            QueueConfig defaultConfig = _configProvider.GetQueueConfig();

            // Populate the SetupViewModel with default values.
            var model = new SetupViewModel
            {
                UnboundedPriority = defaultConfig.UnboundedPriority,
                MaxPriority = defaultConfig.MaxPriority,
                Algorithm = defaultConfig.Algorithm.ToString(),  // Convert enum to string
                UseLogging = defaultConfig.UseLogging,
                UseLocking = defaultConfig.UseLocking,
                UseLazyDelete = defaultConfig.UseLazyDelete,
                UseAnalytics = defaultConfig.UseAnalytics
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult Index(SetupViewModel model)
        {
            // Create a QueueConfig from the submitted model.
            var config = new QueueConfig
            {
                UnboundedPriority = model.UnboundedPriority,
                MaxPriority = model.MaxPriority,
                UseLogging = model.UseLogging,
                UseLocking = model.UseLocking,
                UseLazyDelete = model.UseLazyDelete,
                UseAnalytics = model.UseAnalytics,
                Algorithm = Enum.TryParse<PrioQ.Domain.Entities.PriorityQueueAlgorithm>(model.Algorithm, out var alg)
                            ? alg : PrioQ.Domain.Entities.PriorityQueueAlgorithm.Heap
            };

            // Initialize the queue using your use case.
            _initializeQueueUseCase.Execute(config);

            TempData["Message"] = "Queue created successfully.";
            return RedirectToAction("Index", "Dashboard");
        }
    }
}
