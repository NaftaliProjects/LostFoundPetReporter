

using FirebaseAdmin.Messaging;
using LostFoundPetReporter.CoreDb.Models;
using LostFoundPetReporter.CoreDb.ReposInterfaces;

namespace LostFoundPetReporter.API.Services.Notification
{
    public class FirebasePushNotificationService : IPushNotificationService
    {
        private readonly IUserDeviceRepo _userDeviceRepo;

        public FirebasePushNotificationService(
            IUserDeviceRepo userDeviceRepo)
        {
            _userDeviceRepo = userDeviceRepo;
        }

        public async Task SendMatchNotificationAsync(
            int userId,
            IEnumerable<LostFoundMatch> matches,
            CancellationToken cancellationToken = default)
        {
            var device = _userDeviceRepo.GetByUserId(userId);

            if (device == null)
                return;

            if (string.IsNullOrWhiteSpace(device.Token))
                return;

            var matchCount = matches.Count();

            var message = new FirebaseAdmin.Messaging.Message
            {
                Token = device.Token,

                Notification = new FirebaseAdmin.Messaging.Notification
                {
                    Title = "Possible match found!",
                    Body = matchCount == 1
            ? "We found a possible match for your lost pet."
            : $"We found {matchCount} possible matches for your lost pet."
                },

                Data = new Dictionary<string, string>
                {
                    ["type"] = "lost_report_match"
                }
            };

            await FirebaseMessaging.DefaultInstance.SendAsync(
                message,
                cancellationToken);
        }
    }
}
