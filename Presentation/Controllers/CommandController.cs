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

        // GET: /Command/Enqueue
        public IActionResult Enqueue()
        {
            return View(new CommandViewModel());
        }

        // POST: /Command/Enqueue
        [HttpPost]
        public IActionResult Enqueue(CommandViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var item = new PriorityQueueItem(model.Priority, model.CommandText);

                _enqueueUseCase.Execute(item);
                TempData["Message"] = "Command enqueued successfully.";
                return RedirectToAction("Enqueue");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
        }

        // GET: /Command/Dequeue
        public IActionResult Dequeue()
        {
            // Simply call the use case and pass the result to the view.
            var item = _dequeueUseCase.Execute();
            return View(item);
        }
    }
}
