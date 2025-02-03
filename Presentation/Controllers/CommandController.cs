using Microsoft.AspNetCore.Mvc;
using PrioQ.UI.Models;
using PrioQ.Application.Interfaces;
using PrioQ.Domain.Entities;
using System;

namespace PrioQ.UI.Controllers
{
    public class CommandController : Controller
    {
        private readonly IEnqueueCommandUseCase _enqueueUseCase;
        private readonly IDequeueCommandUseCase _dequeueUseCase;

        public CommandController(IEnqueueCommandUseCase enqueueUseCase, IDequeueCommandUseCase dequeueUseCase)
        {
            _enqueueUseCase = enqueueUseCase;
            _dequeueUseCase = dequeueUseCase;
        }

        // GET: /Command/Manage
        public IActionResult Manage()
        {
            // Initialize the view model as needed.
            var model = new CommandViewModel();
            return View("CommandManagement", model);
        }

        // GET: /Command/DequeueAjax
        [HttpGet]
        public IActionResult DequeueAjax()
        {
            var item = _dequeueUseCase.Execute();
            // Return a partial view that renders the dequeued command.
            return PartialView("_DequeuePartial", item);
        }

        [HttpPost]
        public IActionResult Enqueue(CommandViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Content("Invalid input.");
            }

            try
            {
                var item = new PriorityQueueItem(model.Priority, model.CommandText);
                _enqueueUseCase.Execute(item);
                return Content("Command enqueued successfully.");
            }
            catch (Exception ex)
            {
                return Content("Error: " + ex.Message);
            }
        }


    }
}
