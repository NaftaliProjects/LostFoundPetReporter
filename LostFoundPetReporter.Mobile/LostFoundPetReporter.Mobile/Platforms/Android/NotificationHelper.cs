#if ANDROID

using Android.App;
using Android.Content;
using Android.OS;
using Plugin.Firebase.CloudMessaging;

namespace LostFoundPetReporter.Mobile.Platforms.Android;

public static class NotificationHelper
{
    public static void CreateNotificationChannel()
    {
        if (Build.VERSION.SdkInt < BuildVersionCodes.O)
            return;

        var context = global::Android.App.Application.Context;

        var channelId = $"{context.PackageName}.general";

        var notificationManager =
            (NotificationManager)context
                .GetSystemService(Context.NotificationService)!;

        var channel = new NotificationChannel(
            channelId,
            "General",
            NotificationImportance.High);

        channel.Description =
            "Lost and found pet notifications";

        // Use the phone's default notification sound
        var ringtoneUri =
            global::Android.Media.RingtoneManager.GetActualDefaultRingtoneUri(
                context,
                global::Android.Media.RingtoneType.Notification);

        var audioAttributes =
            new global::Android.Media.AudioAttributes.Builder()
                .SetUsage(global::Android.Media.AudioUsageKind.Notification)
                .SetContentType(
                    global::Android.Media.AudioContentType.Sonification)
                .Build();

        channel.SetSound(ringtoneUri, audioAttributes);

        notificationManager.CreateNotificationChannel(channel);

        FirebaseCloudMessagingImplementation.ChannelId = channelId;
    }
}

#endif