namespace LostFoundPetReporter.API.Services.BackgroundServices
{
    public record ExtFileTask(int ReportId, ReportType Type, List<string> pictureBase64List, CancellationToken cancellationToken = default);

    public interface IExtFileQueue
    {
        ValueTask QueueForExtFileAsync(
            int reportId,
            ReportType type,
            List<string> pictureBase64List);

        ValueTask<ExtFileTask> DequeueAsync(
            CancellationToken cancellationToken);
    }
}
