using Microsoft.AspNetCore.Mvc;
using PrioQ.UI.Models;
using PrioQ.Application.Interfaces;
using PrioQ.Domain.Entities;
using PrioQ.Infrastructure.Configuration; // or your correct namespace

namespace PrioQ.UI.Controllers
{
    public class SetupController : Controller
    {
        private readonly IInitializeQueueUseCase _initializeQueueUseCase;
        private readonly IConfigProvider _configProvider;
        private readonly IQueueRepository _queueRepository; // injected repository

        public SetupController(
            IInitializeQueueUseCase initializeQueueUseCase, 
            IConfigProvider configProvider,
            IQueueRepository queueRepository)
        {
            _initializeQueueUseCase = initializeQueueUseCase;
            _configProvider = configProvider;
            _queueRepository = queueRepository;
        }

        [HttpGet]
        public IActionResult Index()
        {
            // Read default config from config.json
            var defaultConfig = _configProvider.GetQueueConfig();

            // Map default config to SetupViewModel.
            var model = new SetupViewModel
            {
                UnboundedPriority = defaultConfig.UnboundedPriority,
                MaxPriority = defaultConfig.MaxPriority,
                Algorithm = defaultConfig.Algorithm,
                UseLogging = defaultConfig.UseLogging,
                UseLocking = defaultConfig.UseLocking,
                UseLazyDelete = defaultConfig.UseLazyDelete,
                UseAnalytics = defaultConfig.UseAnalytics,
                // Get the server/port dynamically.
                QueueServerInfo = Request.Host.ToString()
            };

            // Check if a queue is currently running in the repository.
            model.QueueIsRunning = _queueRepository.HasQueue();

            if (model.QueueIsRunning)
            {
                model.QueueWarningMessage = "A queue is running on " +
                                              model.QueueServerInfo +
                                              ". Creating a new queue will replace the current queue. " +
                                              "WARNING: Existing items will be lost (item recovery is not implemented).";
            }

            return View(model);
        }

        [HttpPost]
        public IActionResult Index(SetupViewModel model)
        {
            // Reconstruct a new QueueConfig from the model.
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

            // Initialize (or replace) the queue.
            _initializeQueueUseCase.Execute(config);
            
            model.QueueIsRunning = _queueRepository.HasQueue();
            model.QueueServerInfo = Request.Host.ToString();
            if (model.QueueIsRunning)
            {
                model.QueueWarningMessage = "A queue is running on " +
                                              model.QueueServerInfo +
                                              ". Creating a new queue will replace the current queue. " +
                                              "WARNING: Existing items will be lost (item recovery is not implemented).";
            }

            return View(model);
        }
    }
}
