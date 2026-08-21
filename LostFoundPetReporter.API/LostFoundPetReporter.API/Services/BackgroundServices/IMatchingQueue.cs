namespace LostFoundPetReporter.API.Services.BackgroundServices
{
    using System.Threading.Channels;

    public interface IMatchingQueue
    {
        ValueTask QueueReportForMatchingAsync(int foundReportId);
        ValueTask<int> DequeueAsync(CancellationToken cancellationToken);
    }
}
