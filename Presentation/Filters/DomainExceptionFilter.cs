using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using PrioQ.Domain.Exceptions;

namespace PrioQ.Presentation.Filters
{
    public class DomainExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            if (context.Exception is PriorityOutOfRangeException ex)
            {
                context.Result = new BadRequestObjectResult(new
                {
                    Error = ex.Message,
                    ProvidedPriority = ex.ProvidedPriority,
                    MaxAllowed = ex.MaxAllowed
                });
                context.ExceptionHandled = true;
            }
        }
    }
}
