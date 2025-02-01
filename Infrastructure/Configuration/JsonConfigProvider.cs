using System.IO;
using Newtonsoft.Json;
using PrioQ.Domain.Entities;

namespace PrioQ.Infrastructure.Configuration
{
    public class JsonConfigProvider : IConfigProvider
    {
        private readonly string _configFilePath;

        public JsonConfigProvider(string configFilePath)
        {
            _configFilePath = configFilePath;
        }

        public QueueConfig GetQueueConfig()
        {
            var json = File.ReadAllText(_configFilePath);
            return JsonConvert.DeserializeObject<QueueConfig>(json);
        }
    }
}
