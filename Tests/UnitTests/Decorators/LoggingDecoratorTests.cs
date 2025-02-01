using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Xunit;
using PrioQ.Domain.Entities;
using PrioQ.Infrastructure.Decorators;
using PrioQ.Tests.Mocks;

namespace Tests.UnitTests.Decorators
{
    /// <summary>
    /// A simple test logger that collects log messages.
    /// </summary>
    public class TestLogger<T> : ILogger<T>
    {
        /// <summary>
        /// Collected log messages.
        /// </summary>
        public List<string> Logs { get; } = new List<string>();

        public IDisposable BeginScope<TState>(TState state) => NullScope.Instance;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception exception,
            Func<TState, Exception, string> formatter)
        {
            Logs.Add(formatter(state, exception));
        }

        private class NullScope : IDisposable
        {
            public static NullScope Instance { get; } = new NullScope();
            public void Dispose() { }
        }
    }

    public class LoggingDecoratorTests
    {
        [Fact]
        public void Enqueue_LogsMessageAndForwardsCall()
        {
            // Arrange
            var testLogger = new TestLogger<LoggingDecorator>();
            var mockQueue = new MockPriorityQueue();
            var decorator = new LoggingDecorator(mockQueue, testLogger);
            var item = new PriorityQueueItem(1, "Test");

            // Act
            decorator.Enqueue(item);

            // Assert:
            // Verify the inner queue received the item.
            var dequeued = mockQueue.Dequeue();
            Assert.NotNull(dequeued);
            Assert.Equal(item.Id, dequeued.Id);

            // Verify that a log message was captured containing "Enqueueing item"
            Assert.Contains(testLogger.Logs, log => log.Contains("Enqueueing item"));
        }

        [Fact]
        public void Dequeue_LogsMessageAndForwardsCall_WhenItemExists()
        {
            // Arrange
            var testLogger = new TestLogger<LoggingDecorator>();
            var mockQueue = new MockPriorityQueue();
            var decorator = new LoggingDecorator(mockQueue, testLogger);
            var item = new PriorityQueueItem(1, "Test");
            mockQueue.Enqueue(item);

            // Act
            var dequeued = decorator.Dequeue();

            // Assert:
            // Verify the returned item matches the enqueued item.
            Assert.NotNull(dequeued);
            Assert.Equal(item.Id, dequeued.Id);

            // Verify that a log message was captured containing "Dequeued item with Id"
            Assert.Contains(testLogger.Logs, log => log.Contains("Dequeued item with Id"));
        }

        [Fact]
        public void Dequeue_LogsMessageAndForwardsCall_WhenNoItemExists()
        {
            // Arrange
            var testLogger = new TestLogger<LoggingDecorator>();
            var mockQueue = new MockPriorityQueue();
            var decorator = new LoggingDecorator(mockQueue, testLogger);

            // Act
            var dequeued = decorator.Dequeue();

            // Assert:
            // Verify no item is returned.
            Assert.Null(dequeued);

            // Verify that a log message was captured containing "Dequeued null item"
            Assert.Contains(testLogger.Logs, log => log.Contains("Dequeued null item"));
        }
    }
}
