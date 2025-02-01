using PrioQ.Domain.Entities;

namespace PrioQ.Infrastructure.Configuration
{
    public interface IConfigProvider
    {
        QueueConfig GetQueueConfig();
    }
}
