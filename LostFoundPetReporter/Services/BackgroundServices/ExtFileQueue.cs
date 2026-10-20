using System.Threading.Channels;

namespace LostFoundPetReporter.API.Services.BackgroundServices
{

    public class ExtFileQueue : IExtFileQueue
    {
        private readonly Channel<ExtFileTask> _queue = Channel.CreateUnbounded<ExtFileTask>();

        public ValueTask QueueForExtFileAsync(
            int reportId,
            ReportType type,
            List<string> pictureBase64List)
        {
            return _queue.Writer.WriteAsync(
                new ExtFileTask(
                    reportId,
                    type,
                    pictureBase64List));
        }

        public ValueTask<ExtFileTask> DequeueAsync(
            CancellationToken cancellationToken)
        {
            return _queue.Reader.ReadAsync(cancellationToken);
        }
    }
}
