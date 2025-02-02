using System;
using System.IO;
using System.Security.Cryptography;
using Newtonsoft.Json;
using PrioQ.Domain.Entities;
using PrioQ.Infrastructure.Configuration;
using Xunit;

namespace Tests.UnitTests.Configurations
{
    public class JsonConfigProviderTests : IDisposable
    {
        private readonly string _tempFilePath;

        public JsonConfigProviderTests()
        {
            // Create a temporary file path for testing.
            _tempFilePath = Path.GetTempFileName();

            // Copy the content of Presentation/config.json to the temporary file.
            string configFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Presentation", "config.json");
            if (File.Exists(configFilePath))
            {
                File.Copy(configFilePath, _tempFilePath, true);
            }
        }

        [Fact]
        public void GetQueueConfig_ShouldReturnValidConfig_WhenJsonIsValid()
        {
            // Arrange: create a sample QueueConfig object.
            var expectedConfig = new QueueConfig
            {
                UseAnalytics = true,
                UseLogging = true,
                UseLazyDelete = false,
                UseLocking = true,
                Algorithm = PriorityQueueAlgorithm.Bitmask,
            };

            // Serialize the object to JSON and write to the temporary file.
            string json = JsonConvert.SerializeObject(expectedConfig);
            File.WriteAllText(_tempFilePath, json);

            var provider = new JsonConfigProvider(_tempFilePath);

            // Act
            QueueConfig actualConfig = provider.GetQueueConfig();

            // Assert: verify that the deserialized config matches the expected values.
            Assert.NotNull(actualConfig);
            Assert.Equal(expectedConfig.UseAnalytics, actualConfig.UseAnalytics);
            Assert.Equal(expectedConfig.Algorithm, actualConfig.Algorithm);
            Assert.Equal(expectedConfig.UseLogging, actualConfig.UseLogging);
            Assert.Equal(expectedConfig.UseLazyDelete, actualConfig.UseLazyDelete);
            Assert.Equal(expectedConfig.UseLocking, actualConfig.UseLocking);
        }

        [Fact]
        public void GetQueueConfig_ShouldThrowFileNotFoundException_WhenFileDoesNotExist()
        {
            // Arrange: use a non-existent file path.
            string nonExistentFilePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N") + ".json");
            var provider = new JsonConfigProvider(nonExistentFilePath);

            // Act & Assert: expect a FileNotFoundException when attempting to read a missing file.
            Assert.Throws<FileNotFoundException>(() => provider.GetQueueConfig());
        }

        [Fact]
        public void GetQueueConfig_ShouldThrowJsonReaderException_WhenJsonIsInvalid()
        {
            // Arrange: write invalid JSON to the temporary file.
            File.WriteAllText(_tempFilePath, "This is not valid JSON");
            var provider = new JsonConfigProvider(_tempFilePath);

            // Act & Assert: expect a JsonReaderException when attempting to deserialize invalid JSON.
            Assert.Throws<JsonReaderException>(() => provider.GetQueueConfig());
        }

        public void Dispose()
        {
            // Clean up: delete the temporary file if it exists.
            if (File.Exists(_tempFilePath))
                File.Delete(_tempFilePath);
        }
    }
}
