using System;
using System.Collections.Generic;
using Plugin.FirebasePushNotification.Abstractions;

namespace NotificationFirebase.iOS
{
    public class CustomPushNotificationHandler : Plugin.FirebasePushNotification.Abstractions.IPushNotificationHandler
    {
        public CustomPushNotificationHandler()
        {
        }

        public void OnError(string error)
        {
            //throw new NotImplementedException();
        }

        public void OnOpened(NotificationResponse response)
        {
            //throw new NotImplementedException();
        }

        public void OnReceived(IDictionary<string, object> parameters)
        {
            //throw new NotImplementedException();
        }
    }
}
