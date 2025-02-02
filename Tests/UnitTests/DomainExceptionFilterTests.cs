using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Xunit;
using PrioQ.Domain.Exceptions;
using PrioQ.Presentation.Filters;

namespace PrioQ.Tests.UnitTests
{
    public class DomainExceptionFilterTests
    {
        [Fact]
        public void OnException_WithPriorityOutOfRangeException_SetsBadRequestResult()
        {
            // Arrange: Create a minimal ActionContext with a DefaultHttpContext.
            var httpContext = new DefaultHttpContext();
            var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
            var context = new ExceptionContext(actionContext, new List<IFilterMetadata>())
            {
                Exception = new PriorityOutOfRangeException(11, 10)
            };

            var filter = new DomainExceptionFilter();

            // Act
            filter.OnException(context);

            // Assert
            Assert.True(context.ExceptionHandled);
            var result = Assert.IsType<BadRequestObjectResult>(context.Result);
            Assert.Contains("Priority 11 is out of range. It must be between 1 and 10.", result.Value.ToString());
        }
    }
}
