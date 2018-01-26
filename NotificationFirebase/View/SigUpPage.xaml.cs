using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

using NotificationFirebase;
using NotificationFirebase.Model;
using NotificationFirebase.DependencyService;

namespace NotificationFirebase.View
{
    public partial class SigUpPage : ContentPage
    {
        public SigUpPage()
        {
            InitializeComponent();

            MessagingCenter.Subscribe<User>(this, App.MessageSignUp, SignUp);
            MessagingCenter.Subscribe<string>(this, App.MessageSignUpError, SignUpError);
        }

        void OnSignUpButtonClicked(object sender, EventArgs e)
        {
            Xamarin.Forms.DependencyService.Get<IFirebaseManager>().SignUpAsync(emailEntry.Text, passwordEntry.Text);
        }

        async void SignUp(User user)
        {
            // Sign up logic goes here
            var rootPage = Navigation.NavigationStack.FirstOrDefault();
            if (rootPage != null)
            {
                App.IsUserLoggedIn = true;
                //Navigation.InsertPageBefore(new MainPage(), Navigation.NavigationStack.First());
                //await Navigation.PopToRootAsync();
                if (Xamarin.Forms.Device.OS == Xamarin.Forms.TargetPlatform.iOS)
                {
                    await Navigation.PopToRootAsync();
                }
                Application.Current.MainPage = new MainPage();
            }
        }

        void SignUpError(string erorMessage)
        {
            messageLabel.Text = erorMessage;
        }
    }
}
