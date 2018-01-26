using System;
using System.Collections.Generic;

using Xamarin.Forms;
using NotificationFirebase.Model;
using NotificationFirebase.DependencyService;

namespace NotificationFirebase.View
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();

            MessagingCenter.Subscribe<User>(this, App.MessageSignIn, SignIn);
            MessagingCenter.Subscribe<string>(this, App.MessageSignInError, SignInError);
        }

        async void OnSignUpButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SigUpPage());
        }

        void OnLoginButtonClicked(object sender, System.EventArgs e)
        {
            Xamarin.Forms.DependencyService.Get<IFirebaseManager>().SignInAsync(emailEntry.Text, passwordEntry.Text);
        }

        async void SignIn(User user)
        {
            App.IsUserLoggedIn = true;
            //Navigation.InsertPageBefore(new MainPage(), this);
            //await Navigation.PopAsync();
            if (Xamarin.Forms.Device.OS == Xamarin.Forms.TargetPlatform.iOS)
            {
                await Navigation.PopToRootAsync();
            }
            Application.Current.MainPage = new MainPage();
        }

        void SignInError(string erorMessage)
        {
            messageLabel.Text = erorMessage;
        }
    }
}
