namespace LostFoundPetReporter.API.Services.Notification
{
    public interface IPushNotificationService
    {
        Task SendMatchNotificationAsync(IEnumerable<int> userIds, IEnumerable<LostFoundMatch> matches, CancellationToken cancellationToken = default);
    }
}
