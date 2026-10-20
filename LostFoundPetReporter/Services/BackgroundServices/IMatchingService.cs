namespace LostFoundPetReporter.API.Services.BackgroundServices
{
    public interface IMatchingService
    {
        Task TryMatchFoundReportAsync(int foundReportId, CancellationToken cancellationToken = default);
        Task TryMatchLostReportAsync(int lostReportId, CancellationToken cancellationToken = default);
    }
}
