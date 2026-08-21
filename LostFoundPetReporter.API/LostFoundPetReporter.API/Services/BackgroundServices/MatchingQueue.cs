using System.Threading.Channels;

namespace LostFoundPetReporter.API.Services.BackgroundServices
{
    public class MatchingQueue : IMatchingQueue
    {
        private readonly Channel<int> _queue = Channel.CreateUnbounded<int>();

        public ValueTask QueueReportForMatchingAsync(int foundReportId)
            => _queue.Writer.WriteAsync(foundReportId);

        public ValueTask<int> DequeueAsync(CancellationToken cancellationToken)
            => _queue.Reader.ReadAsync(cancellationToken);
    }
}
