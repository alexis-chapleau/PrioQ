using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Mvc;
using PrioQ.Domain.Entities;
using PrioQ.Application.Interfaces;

namespace PrioQ.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PriorityQueueController : ControllerBase
    {
        private readonly IEnqueueCommandUseCase _enqueueUseCase;
        private readonly IDequeueCommandUseCase _dequeueUseCase;
        private readonly IInitializeQueueUseCase _initializeUseCase;

        public PriorityQueueController(
            IEnqueueCommandUseCase enqueueUseCase,
            IDequeueCommandUseCase dequeueUseCase,
            IInitializeQueueUseCase initializeUseCase)
        {
            _enqueueUseCase = enqueueUseCase;
            _dequeueUseCase = dequeueUseCase;
            _initializeUseCase = initializeUseCase;
        }

        [HttpPost("enqueue")]
        public IActionResult Enqueue([FromBody] PriorityQueueItem item)
        {
            _enqueueUseCase.Execute(item);
            return Ok("Item enqueued.");
        }

        [HttpPost("dequeue")]
        public IActionResult Dequeue()
        {
            var item = _dequeueUseCase.Execute();
            if (item == null)
                return NotFound("Queue is empty.");
            return Ok(item);
        }

        [HttpPost("initialize")]
        public IActionResult Reinitialize()
        {
            _initializeUseCase.Execute();
            return Ok("Queue reinitialized.");
        }
    }
}
