using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;

namespace PrioQ.Tests.StressTests
{
    public class ConcurrentProducersAndConsumersStressTest : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public ConcurrentProducersAndConsumersStressTest(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Many_Concurrent_Producers_And_Consumers_Results_In_Processed_Items()
        {
            // Configure stress test parameters:
            int numberOfProducers = 100;       // e.g., 100 concurrent producers
            int commandsPerProducer = 50;      // each producer sends 50 commands
            int totalCommands = numberOfProducers * commandsPerProducer;
            var enqueueUrl = "/api/priorityqueue/enqueue";
            var dequeueUrl = "/api/priorityqueue/dequeue";

            // We'll use the current time for enqueuedAt.
            // Use ISO 8601 format for consistency.
            string GetCurrentTimeString() => DateTime.UtcNow.ToString("o");

            // JSON template with a placeholder for enqueuedAt.
            string jsonTemplate = @"{{
                ""priority"": {0},
                ""command"": ""StressTest Command {1}"",
                ""enqueuedAt"": ""{2}""
            }}";

            // Launch producer tasks.
            var producerTasks = new List<Task>();
            for (int p = 0; p < numberOfProducers; p++)
            {
                producerTasks.Add(Task.Run(async () =>
                {
                    for (int i = 0; i < commandsPerProducer; i++)
                    {
                        int priority = (p % 5) + 1; // priorities 1 to 5
                        string commandText = $"Producer {p} Command {i}";
                        // Use current time for enqueuedAt.
                        string enqueuedAt = GetCurrentTimeString();
                        string json = string.Format(jsonTemplate, priority, commandText, enqueuedAt);

                        using (var content = new StringContent(json, Encoding.UTF8, "application/json"))
                        {
                            HttpResponseMessage response = await _client.PostAsync(enqueueUrl, content);
                            response.EnsureSuccessStatusCode();
                        }
                    }
                }));
            }
            await Task.WhenAll(producerTasks);

            // Launch consumer tasks to process items.
            // We'll start a fixed number of consumer tasks that repeatedly call the dequeue endpoint.
            int numberOfConsumers = 20; // Adjust as needed.
            var consumerTasks = new List<Task<int>>(); // Each returns the count of dequeued items.
            for (int c = 0; c < numberOfConsumers; c++)
            {
                consumerTasks.Add(Task.Run(async () =>
                {
                    int localCount = 0;
                    while (true)
                    {
                        var response = await _client.PostAsync(dequeueUrl, null);
                        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                        {
                            // Queue is empty.
                            break;
                        }
                        response.EnsureSuccessStatusCode();
                        localCount++;
                    }
                    return localCount;
                }));
            }
            var consumerResults = await Task.WhenAll(consumerTasks);
            int totalDequeued = 0;
            foreach (var count in consumerResults)
            {
                totalDequeued += count;
            }
            Console.WriteLine($"Total dequeued: {totalDequeued} (of {totalCommands} enqueued)");

            // Now, set a report query window that includes the current time.
            // For example, from 5 minutes ago to 5 minutes in the future.
            DateTime reportFrom = DateTime.UtcNow.AddMinutes(-5);
            DateTime reportTo = DateTime.UtcNow.AddMinutes(5);
            var reportUrl = $"/api/reports?from={reportFrom:o}&to={reportTo:o}";

            // Request the report.
            var reportResponse = await _client.GetAsync(reportUrl);
            reportResponse.EnsureSuccessStatusCode();
            var reportJson = await reportResponse.Content.ReadAsStringAsync();

            // Assert that the report indicates some items were processed.
            Assert.DoesNotContain("\"totalItemsProcessed\":0", reportJson, StringComparison.OrdinalIgnoreCase);
        }
    }
}
