namespace LostFoundPetReporter.API.Services.BackgroundServices
{
    using System.Threading.Channels;

    public enum ReportType
    {
        Found,
        Lost
    }

    public record ReportMatchingTask(int ReportId, ReportType Type);


    public interface IMatchingQueue
    {
        ValueTask QueueForMatchingAsync(int reportId, ReportType type);
        ValueTask<ReportMatchingTask> DequeueAsync(CancellationToken cancellationToken = default);
    }
}
