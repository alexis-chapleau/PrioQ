using PrioQ.Application.DTOs;
using PrioQ.Domain.Entities;

namespace PrioQ.Application.Interfaces
{
    public interface IEnqueueCommandUseCase
    {
        void Execute(PriorityQueueItem item);
    }

    public interface IDequeueCommandUseCase
    {
        PriorityQueueItem Execute();
    }

    public interface IInitializeQueueUseCase
    {
        void Execute();
    }

    public interface IAnalyticsReportUseCase
    {
        AnalyticsReportDto GenerateReport(DateTime from, DateTime to);
    }
}
