using System;
using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using PrioQ.Domain.Entities;
using PrioQ.Application.Interfaces;
using PrioQ.Presentation.Controllers;

namespace Tests.UnitTests.Controllers
{
    public class PriorityQueueControllerTests
    {
        [Fact]
        public void Enqueue_ReturnsOkResult_WithMessage()
        {
            // Arrange
            var mockEnqueueUseCase = new Mock<IEnqueueCommandUseCase>();
            var mockDequeueUseCase = new Mock<IDequeueCommandUseCase>();
            var mockInitializeUseCase = new Mock<IInitializeQueueUseCase>();

            // Create the controller with the mocked use cases.
            var controller = new PriorityQueueController(
                mockEnqueueUseCase.Object,
                mockDequeueUseCase.Object,
                mockInitializeUseCase.Object);

            // Create a sample item.
            var item = new PriorityQueueItem(1, "Test Enqueue");

            // Act
            var result = controller.Enqueue(item);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Item enqueued.", okResult.Value);
            // Verify that the enqueue use case was called once with the correct item.
            mockEnqueueUseCase.Verify(x => x.Execute(item), Times.Once);
        }

        [Fact]
        public void Dequeue_ReturnsOkResult_WithItem_WhenItemExists()
        {
            // Arrange
            var expectedItem = new PriorityQueueItem(2, "Test Dequeue");
            var mockEnqueueUseCase = new Mock<IEnqueueCommandUseCase>();
            var mockDequeueUseCase = new Mock<IDequeueCommandUseCase>();
            var mockInitializeUseCase = new Mock<IInitializeQueueUseCase>();

            // Setup the dequeue use case to return the expected item.
            mockDequeueUseCase.Setup(x => x.Execute()).Returns(expectedItem);

            var controller = new PriorityQueueController(
                mockEnqueueUseCase.Object,
                mockDequeueUseCase.Object,
                mockInitializeUseCase.Object);

            // Act
            var result = controller.Dequeue();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedItem = Assert.IsType<PriorityQueueItem>(okResult.Value);
            Assert.Equal(expectedItem.Id, returnedItem.Id);
        }

        [Fact]
        public void Dequeue_ReturnsNotFoundResult_WhenQueueIsEmpty()
        {
            // Arrange
            var mockEnqueueUseCase = new Mock<IEnqueueCommandUseCase>();
            var mockDequeueUseCase = new Mock<IDequeueCommandUseCase>();
            var mockInitializeUseCase = new Mock<IInitializeQueueUseCase>();

            // Setup the dequeue use case to return null.
            mockDequeueUseCase.Setup(x => x.Execute()).Returns((PriorityQueueItem)null);

            var controller = new PriorityQueueController(
                mockEnqueueUseCase.Object,
                mockDequeueUseCase.Object,
                mockInitializeUseCase.Object);

            // Act
            var result = controller.Dequeue();

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Queue is empty.", notFoundResult.Value);
        }

        [Fact]
        public void Reinitialize_ReturnsOkResult_WithMessage()
        {
            // Arrange
            var mockEnqueueUseCase = new Mock<IEnqueueCommandUseCase>();
            var mockDequeueUseCase = new Mock<IDequeueCommandUseCase>();
            var mockInitializeUseCase = new Mock<IInitializeQueueUseCase>();

            var controller = new PriorityQueueController(
                mockEnqueueUseCase.Object,
                mockDequeueUseCase.Object,
                mockInitializeUseCase.Object);

            // Act
            var result = controller.Initialize();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Queue reinitialized.", okResult.Value);
            mockInitializeUseCase.Verify(x => x.Execute(), Times.Once);
        }
    }
}
