namespace LostFoundPetReporter.API.Services.BackgroundServices
{
    public interface IExtFileService
    {
        Task ProcessFilesAsync(
            int reportId,
            ReportType type,
            List<string> pictureBase64List,
            CancellationToken cancellationToken = default);
    }
}
