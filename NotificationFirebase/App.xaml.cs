using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Plugin.FirebasePushNotification;
using Plugin.FirebasePushNotification.Abstractions;

using NotificationFirebase.View;
using NotificationFirebase.Model;
using System;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace NotificationFirebase
{
    public partial class App : Application
    {
        public static bool IsUserLoggedIn { get; set; }
        public static User user { get; set; }
        public const string MessageSignIn       = "SignIn";
        public const string MessageSignInError  = "SignInError";
        public const string MessageSignUp       = "SignUp";
        public const string MessageSignUpError  = "SignUpError";
        public const string MessageSignOut      = "SignOut";
        public const string MessageSignOutError = "SignOutError";

        public App()
        {
            InitializeComponent();

            IsUserLoggedIn = Xamarin.Forms.DependencyService.Get<DependencyService.IFirebaseManager>().IsUserLoggedIn();

            if(IsUserLoggedIn)
            {
                MainPage = new MainPage();
            }
            else
            {
                //MainPage = new NavigationPage(new LoginPage());
                MainPage = new LoginPage();
            }
        }

        protected override void OnStart()
        {
            // Handle when your app starts
            CrossFirebasePushNotification.Current.OnTokenRefresh += Current_OnTokenRefresh;
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }

        void Current_OnTokenRefresh(object source, FirebasePushNotificationTokenEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"TOKEN: {CrossFirebasePushNotification.Current.Token}");

            CrossFirebasePushNotification.Current.Subscribe("general");

            System.Diagnostics.Debug.WriteLine($"TOKEN REC: {e.Token}");
        }
    }
}
