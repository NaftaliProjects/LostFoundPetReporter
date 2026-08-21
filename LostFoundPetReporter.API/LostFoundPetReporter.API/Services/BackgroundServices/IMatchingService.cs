namespace LostFoundPetReporter.API.Services.BackgroundServices
{
    public interface IMatchingService
    {
        Task TryMatchLostReportAsync(int foundReportId, CancellationToken cancellationToken = default);
    }
}
