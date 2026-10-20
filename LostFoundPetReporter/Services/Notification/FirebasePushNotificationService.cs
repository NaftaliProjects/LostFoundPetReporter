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
            IEnumerable<int> userIds,
            IEnumerable<LostFoundMatch> matches,
            CancellationToken cancellationToken = default)
        {
            var uniqueUserIds = userIds.Distinct().ToList();

            var matchList = matches.ToList();

            if (uniqueUserIds.Count == 0 || matchList.Count == 0)
                return;

            var matchCount = matchList.Count;

            foreach (var userId in uniqueUserIds)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var device = _userDeviceRepo.GetByUserId(userId);

                if (device == null)
                    continue;

                if (string.IsNullOrWhiteSpace(device.Token))
                    continue;

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

                var response = await FirebaseMessaging.DefaultInstance.SendAsync(message, cancellationToken);



                Console.WriteLine(
                    $"================ FCM SENT ================");

                Console.WriteLine(
                    $"UserId: {userId}");

                Console.WriteLine(
                    $"Token: {device.Token}");

                Console.WriteLine(
                    $"FCM Message ID: {response}");

                Console.WriteLine(
                    $"===========================================");
            }
        }
    }
}