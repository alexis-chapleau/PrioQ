﻿using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Mvc;
using PrioQ.Application.UseCases;
using PrioQ.Domain.Entities;
using PrioQ.Application.Interfaces;

namespace PrioQ.Presentation.API
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
            try
            {
                _enqueueUseCase.Execute(item);
                return Ok("Item enqueued.");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
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
        public IActionResult Initialize()
        {
            _initializeUseCase.Execute();
            return Ok("Queue reinitialized.");
        }
    }
}
