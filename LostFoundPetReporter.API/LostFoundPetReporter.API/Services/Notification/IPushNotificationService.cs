namespace LostFoundPetReporter.API.Services.Notification
{
    public interface IPushNotificationService
    {
        Task SendMatchNotificationAsync(int userId, IEnumerable<LostFoundMatch> matches, CancellationToken cancellationToken = default);
    }
}
