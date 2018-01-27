using System;
using System.Collections.Generic;

using Xamarin.Forms;

using Plugin.FirebasePushNotification;
using Plugin.FirebasePushNotification.Abstractions;

using NotificationFirebase;
using NotificationFirebase.DependencyService;

namespace NotificationFirebase.View
{
    public partial class HomePage : ContentPage
    {
        public HomePage()
        {
            InitializeComponent();

            CrossFirebasePushNotification.Current.OnNotificationReceived += Current_OnNotificationReceived;
            CrossFirebasePushNotification.Current.OnNotificationOpened += Current_OnNotificationOpened;
            CrossFirebasePushNotification.Current.OnNotificationError += Current_OnNotificationError;
        }

        void Current_OnNotificationReceived(object source, FirebasePushNotificationDataEventArgs e)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("Received");

                if(e.Data.ContainsKey("body"))
                {
                    Device.BeginInvokeOnMainThread(()=>{
                        lblMessage.Text = $"{e.Data["body"]}";
                    });
                }
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Excepton on Received:" + ex.Message);
            }
        }

        void Current_OnNotificationOpened(object source, FirebasePushNotificationResponseEventArgs e)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("Opened");

                foreach(var data in e.Data)
                {
                    System.Diagnostics.Debug.WriteLine($"{data.Key} : {data.Value}");
                }

                if(!string.IsNullOrEmpty(e.Identifier))
                {
                    System.Diagnostics.Debug.WriteLine($"ActionId: {e.Identifier}");

                    Device.BeginInvokeOnMainThread(()=>{
                        lblMessage.Text = e.Identifier;
                    });
                }
                else if(e.Data.ContainsKey("aps.alert.title"))
                {
                    Device.BeginInvokeOnMainThread(()=>{
                        lblMessage.Text = $"{e.Data["aps.alert.title"]}";
                    });
                }
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Excepton on Opened:" + ex.Message);
            }
        }

        void Current_OnNotificationError(object source, FirebasePushNotificationErrorEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Opened");

            if(e != null && !string.IsNullOrWhiteSpace(e.Message))
                System.Diagnostics.Debug.WriteLine("Error:" + e.Message);
        }

        void Handle_Clicked(object sender, System.EventArgs e)
        {
            Xamarin.Forms.DependencyService.Get<IFirebaseManager>().SendNotificationToDeviceAsync("Prueba desde la app general", CrossFirebasePushNotification.Current.Token);
        }

        void Handle_ClickedChannel(object sender, System.EventArgs e)
        {
            Xamarin.Forms.DependencyService.Get<IFirebaseManager>().SendNotificationToChannelAsync("Prueba desde la app general", "general");
        }
        void Handle_ClickedUsers(object sender, System.EventArgs e)
        {
            double myLatitude = 0.0f;
            double myLongitude = 0.0f;
            Xamarin.Forms.DependencyService.Get<IFirebaseManager>().SendNotificationToNearestDevices(5.0f, myLatitude, myLongitude);
        }
    }
}
