using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;

using Plugin.FirebasePushNotification;
using Firebase.Auth;



namespace NotificationFirebase.iOS
{
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        Foundation.NSObject authListenerHandle;
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();

            // Code for starting up the Xamarin Test Cloud Agent
#if DEBUG
            Xamarin.Calabash.Start();
#endif

            LoadApplication(new App());

            FirebasePushNotificationManager.Initialize(options, true);

            FirebasePushNotificationManager.Initialize(options, new CustomPushNotificationHandler());

            authListenerHandle = Auth.DefaultInstance.AddAuthStateDidChangeListener((auth, user) => {
                if (user != null)
                {
                    // User is signed in.
                    // an user is already signin
                    if (App.IsUserLoggedIn)
                    {
                        App.IsUserLoggedIn = true;
                        //MessagingCenter.Send(App.user, NotificationFirebase.App.MessageSignIn);
                    }
                    System.Console.WriteLine("onAuthStateChanged:signed_in:" + user.Uid);
                }
                else
                {
                    // No user is signed in.
                    if (App.IsUserLoggedIn)
                    {
                        App.IsUserLoggedIn = false;
                        //MessagingCenter.Send("SignOut", NotificationFirebase.App.MessageSignOut);
                    }
                }
            });


            return base.FinishedLaunching(app, options);
        }

        public override void RegisteredForRemoteNotifications(UIApplication application, NSData deviceToken)
        {
            base.RegisteredForRemoteNotifications(application, deviceToken);
#if DEBUG
            FirebasePushNotificationManager.DidRegisterRemoteNotifications(deviceToken, FirebaseTokenType.Sandbox);
#else
            FirebasePushNotificationManager.DidRegisterRemoteNotifications(deviceToken, FirebaseTokenType.Production);
#endif
        }

        public override void FailedToRegisterForRemoteNotifications(UIApplication application, NSError error)
        {
            base.FailedToRegisterForRemoteNotifications(application, error);
            FirebasePushNotificationManager.RemoteNotificationRegistrationFailed(error);
        }

        // To receive notifications in foregroung on iOS 9 and below.
        // To receive notifications in background in any iOS version
        public override void DidReceiveRemoteNotification(UIApplication application, NSDictionary userInfo, Action<UIBackgroundFetchResult> completionHandler)
        {
            //base.DidReceiveRemoteNotification(application, userInfo, completionHandler);

            // If you are receiving a notification message while your app is in the background,
            // this callback will not be fired 'till the user taps on the notification launching the application.

            // If you disable method swizzling, you'll need to call this method. 
            // This lets FCM track message delivery and analytics, which is performed
            // automatically with method swizzling enabled.
            //FirebasePushNotificationManager.DidReceiveMessage(userInfo);
            // Do your magic to handle the notification data
            System.Console.WriteLine(userInfo);
        }

        public override void OnActivated(UIApplication uiApplication)
        {
            base.OnActivated(uiApplication);
            FirebasePushNotificationManager.Connect();
        }

        public override void DidEnterBackground(UIApplication uiApplication)
        {
            // base.DidEnterBackground(uiApplication);
            // Use this method to release shared resources, save user data, invalidate timers and store the application state.
            // If your application supports background exection this method is called instead of WillTerminate when the user quits.
            FirebasePushNotificationManager.Disconnect();
        }
    }
}
