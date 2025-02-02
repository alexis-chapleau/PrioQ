using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using Microsoft.AspNetCore.Hosting;

namespace PrioQ.Tests.EndToEndTests
{
    public class UserWorkflowTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public UserWorkflowTests()
        {
            var factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Testing");
            });
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task User_Can_Enqueue_And_Generate_Report()
        {
            // Simulate a user enqueuing several items.
            var enqueueUrl = "/api/priorityqueue/enqueue";
            for (int i = 0; i < 5; i++)
            {
                var itemJson = $@"{{
                    ""priority"": 1,
                    ""command"": ""Item {i}"",
                    ""enqueuedAt"": ""2025-01-01T00:00:00Z""
                }}";
                var content = new StringContent(itemJson, System.Text.Encoding.UTF8, "application/json");
                var response = await _client.PostAsync(enqueueUrl, content);
                response.EnsureSuccessStatusCode();
            }

            // Then generate a report.
            var from = "2025-01-01T00:00:00Z";
            var to = "2025-01-08T00:00:00Z";
            var reportUrl = $"/api/reports?from={from}&to={to}";
            var reportResponse = await _client.GetAsync(reportUrl);
            reportResponse.EnsureSuccessStatusCode();
            var reportJson = await reportResponse.Content.ReadAsStringAsync();

            // Verify that the report contains expected keys (case-insensitive).
            Assert.Contains("averageTimeInQueueByPriority", reportJson, System.StringComparison.OrdinalIgnoreCase);
        }
    }
}
