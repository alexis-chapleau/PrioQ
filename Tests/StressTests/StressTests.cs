using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Hosting;

namespace PrioQ.Tests.StressTests
{
    public class StressTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public StressTests()
        {
            var factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Testing");
            });
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task StressTest_MultipleConcurrentGetReport_Requests()
        {
            int concurrentRequests = 100; // Adjust this value as needed
            var tasks = new List<Task<HttpResponseMessage>>();
            var from = "2025-01-01T00:00:00Z";
            var to = "2025-01-08T00:00:00Z";
            var url = $"/api/reports?from={from}&to={to}";

            for (int i = 0; i < concurrentRequests; i++)
            {
                tasks.Add(_client.GetAsync(url));
            }

            var responses = await Task.WhenAll(tasks);
            foreach (var response in responses)
            {
                // If the response is not successful, capture its content and throw an exception to see the error.
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Request failed with status code {response.StatusCode}: {errorContent}");
                }
            }
        }
    }
}
