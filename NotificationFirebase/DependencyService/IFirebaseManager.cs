using System;
using System.Collections.Generic;
using NotificationFirebase.Model;

namespace NotificationFirebase.DependencyService
{
    public interface IFirebaseManager
    {
        void SendNotificationToDeviceAsync(string message, string tokenDevice);
        void SendNotificationToChannelAsync(string message, string channel);

        void UpdateUserDeviceToken(); //string userid, string deviceToken);
        List<NotificationFirebase.Model.User> GetUsers();
        List<string> GetDeviceTokenUserListFromDistance(double distance, List<NotificationFirebase.Model.User> users);

        void SendNotificationToNearestDevices(double maxDistance, double myLatitude, double myLongitude);

        void SignInAsync(string email, string password);
        void SignUpAsync(string email, string password);
        void SignOut();

        void SendChatMessage(string message);

        User GetCurrentUser();
        bool IsUserLoggedIn();
    }
}
