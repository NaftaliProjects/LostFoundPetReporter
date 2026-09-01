#if ANDROID
using Plugin.Firebase.CloudMessaging;
using Plugin.Firebase.CloudMessaging.EventArgs;
#endif

namespace LostFoundPetReporter.Mobile.Services.Notification
{
    public class PushNotificationService
    {
#if ANDROID
        private readonly IFirebaseCloudMessaging _messaging;
#endif

        public PushNotificationService()
        {
#if ANDROID
            _messaging = CrossFirebaseCloudMessaging.Current;

            _messaging.NotificationReceived += OnNotificationReceived;
            _messaging.NotificationTapped += OnNotificationTapped;
            _messaging.TokenChanged += OnTokenChanged;
            _messaging.Error += OnFirebaseError;
#endif
        }

#if ANDROID
        private void OnNotificationReceived(
        object? sender,
        FCMNotificationReceivedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(
                "========== FCM NOTIFICATION RECEIVED ==========");

            if (e.Notification != null)
            {
                System.Diagnostics.Debug.WriteLine(
                    $"Title: {e.Notification.Title}");

                System.Diagnostics.Debug.WriteLine(
                    $"Body: {e.Notification.Body}");

                if (e.Notification.Data != null)
                {
                    foreach (var item in e.Notification.Data)
                    {
                        System.Diagnostics.Debug.WriteLine(
                            $"Data: {item.Key} = {item.Value}");
                    }
                }
            }

            System.Diagnostics.Debug.WriteLine(
                "================================================");
        }

        private void OnNotificationTapped(
        object? sender,
        FCMNotificationTappedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(
                "========== FCM NOTIFICATION TAPPED ==========");

            if (e.Notification != null)
            {
                System.Diagnostics.Debug.WriteLine(
                    $"Title: {e.Notification.Title}");

                System.Diagnostics.Debug.WriteLine(
                    $"Body: {e.Notification.Body}");
            }

            System.Diagnostics.Debug.WriteLine(
                "==============================================");
        }

        private void OnTokenChanged(
            object? sender,
            FCMTokenChangedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(
                "========== FCM TOKEN CHANGED ==========");

            System.Diagnostics.Debug.WriteLine(
                $"Token: {e}");

            System.Diagnostics.Debug.WriteLine(
                "=======================================");
        }

        private void OnFirebaseError(
            object? sender,
            FCMErrorEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(
                "========== FCM ERROR ==========");

            System.Diagnostics.Debug.WriteLine(
                $"Error: {e}");

            System.Diagnostics.Debug.WriteLine(
                "===============================");
        }
#endif

        public async Task<string?> GetTokenAsync()
        {
#if ANDROID
            var token = await _messaging.GetTokenAsync();

            System.Diagnostics.Debug.WriteLine(
                "================ FCM TOKEN ================");

            System.Diagnostics.Debug.WriteLine(token);

            System.Diagnostics.Debug.WriteLine(
                "============================================");

            return token;
#else
            return null;
#endif
        }
    }
}