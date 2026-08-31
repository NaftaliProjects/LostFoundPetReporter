#if ANDROID
using Plugin.Firebase.CloudMessaging;
#endif
using System;
using System.Collections.Generic;
using System.Text;

namespace LostFoundPetReporter.Mobile.Services.Notification
{
    public class PushNotificationService
    {
        public async Task<string?> GetTokenAsync()
        {
            #if ANDROID

                var token = await CrossFirebaseCloudMessaging.Current.GetTokenAsync();

                return token;
            #endif

            return "";
        }
    }
}
