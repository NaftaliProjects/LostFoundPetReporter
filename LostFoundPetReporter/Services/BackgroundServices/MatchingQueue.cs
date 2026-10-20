using System.Threading.Channels;

namespace LostFoundPetReporter.API.Services.BackgroundServices
{
    public class MatchingQueue : IMatchingQueue
    {
        private readonly Channel<ReportMatchingTask> _queue = Channel.CreateUnbounded<ReportMatchingTask>();

        public ValueTask QueueForMatchingAsync(int reportId, ReportType type)
            => _queue.Writer.WriteAsync(new ReportMatchingTask(reportId, type));

        public ValueTask<ReportMatchingTask> DequeueAsync(CancellationToken cancellationToken)
            => _queue.Reader.ReadAsync(cancellationToken);
    }
}
