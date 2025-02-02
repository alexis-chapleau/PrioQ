using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;

namespace PrioQ.Tests.IntegrationTests
{
    // The WebApplicationFactory<T> uses the Program class (which we made partial and public).
    public class ApiIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public ApiIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetReport_Endpoint_ReturnsOkAndJsonContent()
        {
            // Arrange
            var from = "2025-01-01T00:00:00Z";
            var to = "2025-01-08T00:00:00Z";
            var url = $"/api/reports?from={from}&to={to}";

            // Act
            var response = await _client.GetAsync(url);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal("application/json; charset=utf-8",
                         response.Content.Headers.ContentType.ToString());
            var json = await response.Content.ReadAsStringAsync();
            // Check that the returned JSON contains the expected property (case-insensitive).
            Assert.Contains("averageTimeInQueueByPriority", json, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task Enqueue_And_Dequeue_Work_EndToEnd()
        {
            // Arrange
            // Ensure the queue is initialized first.
            // If you have an endpoint for initialization, call it:
            var initUrl = "/api/priorityqueue/initialize"; // Adjust this route to match your actual endpoint.
            var initResponse = await _client.PostAsync(initUrl, null);
            initResponse.EnsureSuccessStatusCode();

            var enqueueUrl = "/api/priorityqueue/enqueue";
            var dequeueUrl = "/api/priorityqueue/dequeue";

            // Create a sample item (JSON representation).
            var sampleItemJson = @"{
                ""priority"": 1,
                ""command"": ""Test EndToEnd"",
                ""enqueuedAt"": ""2025-01-01T00:00:00Z""
                }";
            var content = new StringContent(sampleItemJson, Encoding.UTF8, "application/json");

            // Act - Enqueue the item.
            var enqueueResponse = await _client.PostAsync(enqueueUrl, content);
            enqueueResponse.EnsureSuccessStatusCode();
            var enqueueResult = await enqueueResponse.Content.ReadAsStringAsync();
            Assert.Contains("Item enqueued.", enqueueResult);

            // Act - Dequeue the item.
            var dequeueResponse = await _client.PostAsync(dequeueUrl, null);
            dequeueResponse.EnsureSuccessStatusCode();
            var dequeueResult = await dequeueResponse.Content.ReadAsStringAsync();
            Assert.Contains("Test EndToEnd", dequeueResult);
        }

    }
}
